using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Game : MonoBehaviour
{
    [SerializeField]
    Tilemap tilemap;

    [SerializeField]
    private Tile foodTile, floorTile, snakeTile;

    [SerializeField]
    private Snake snake; //USELESS?

    [SerializeField]
    private GameObject pauseCanvas;

    [SerializeField]
    private Camera cam;

    public Vector3Int foodPosition;

    public event Action EnterPause;
    public event Action ExitPause;

    private void Awake()
    {
        // Register to event
        snake.AteFood += OnSnakeAteFood;
        snake.ExecutedMove += OnSnakeExecutedMove;
        snake.Died += OnSnakeDied;
        snake.FinishedGameOverSequence += OnSnakeFinishedGameOverSequence;
        foodPosition = new Vector3Int(1, -2, 1);

        // Sound
        SfxManager sfxManager = FindAnyObjectByType<SfxManager>();

        if (sfxManager == null) return;

        EnterPause += sfxManager.OnEnterPause;
        ExitPause += sfxManager.OnExitPause;

        // Music
        MusicManager musicManager = FindAnyObjectByType<MusicManager>();

        if (musicManager == null) return;

        EnterPause += musicManager.OnEnterPause;
        ExitPause += musicManager.OnExitPause;
    }

    private void OnSnakeDied()
    {
        UpdateTilemap();
        StartCoroutine(cam.GetComponent<CameraShake>().Shake());
    }

    private void OnDestroy()
    {
        snake.AteFood -= OnSnakeAteFood;
        snake.ExecutedMove -= OnSnakeExecutedMove;
        snake.Died -= OnSnakeDied;
        snake.FinishedGameOverSequence -= OnSnakeFinishedGameOverSequence;

        // Sound
        SfxManager sfxManager = FindAnyObjectByType<SfxManager>();

        if (sfxManager == null) return;

        EnterPause -= sfxManager.OnEnterPause;
        ExitPause -= sfxManager.OnExitPause;

        // Music
        MusicManager musicManager = FindAnyObjectByType<MusicManager>();

        if (musicManager == null) return;

        EnterPause -= musicManager.OnEnterPause;
        ExitPause -= musicManager.OnExitPause;
    }
    void Start()
    {
        UpdateTilemap();
    }

    public void UpdateTilemap()
    {
        ClearLayer(1);
        tilemap.SetTile(foodPosition, foodTile);
        snake.DrawInTilemap();
    }
    private void GenerateFood()
    {
        var freeTileCoordinates = new List<Vector3Int>();
        for(int x = tilemap.cellBounds.min.x; x < tilemap.cellBounds.max.x; x++)
        {
            for(int y = tilemap.cellBounds.min.y; y < tilemap.cellBounds.max.y; y++)
            {
                Vector3Int coord0 = new Vector3Int(x, y, 0);
                Vector3Int coord1 = new Vector3Int(x, y, 1);
                TileBase t0 = tilemap.GetTile(coord0);
                TileBase t1 = tilemap.GetTile(coord1);
                
                if (t0 == floorTile && t1 == null) freeTileCoordinates.Add(coord1);
            }
        }

        foodPosition = freeTileCoordinates[UnityEngine.Random.Range(0, freeTileCoordinates.Count)];        
    }

    private void GenerateFood2()
    {
        var freeTileCoordinates = new List<Vector2Int>();
        for (int x = -5; x < 4; x++)
        {
            for (int y = -4; y < 4; y++)
            {
                Vector2Int coord = new Vector2Int(x, y);
                freeTileCoordinates.Add(coord);
            }
        }
        for (int i = 0; i < snake.parts.Count - 1; i++)
        {
            freeTileCoordinates.Remove(snake.parts[i]);
        }

        Vector2Int pos = freeTileCoordinates[UnityEngine.Random.Range(0, freeTileCoordinates.Count - 1)];
        foodPosition = new Vector3Int(pos.x, pos.y, 1);
    }

    private void ClearLayer(int layer)
    {
        BoundsInt area = new BoundsInt(tilemap.cellBounds.min.x, tilemap.cellBounds.min.y, layer, tilemap.size.x, tilemap.size.y, 1);
        TileBase[] tileArray = new TileBase[area.size.x * area.size.y];
        for (int index = 0; index < tileArray.Length; index++)
        {
            tileArray[index] = null;
        }
        tilemap.SetTilesBlock(area, tileArray);
    }

    private void OnSnakeAteFood()
    {
        GenerateFood2();
    }

    private void OnSnakeExecutedMove()
    {
        UpdateTilemap();
    }
    private void OnSnakeFinishedGameOverSequence()
    {
        UpdateTilemap();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            snake.paused = true;
            pauseCanvas.SetActive(true);
            EnterPause?.Invoke();
        }
    }

    public void ResumeGame()
    {
        snake.paused = false;
        pauseCanvas.SetActive(false);
        ExitPause?.Invoke();
    }
}
