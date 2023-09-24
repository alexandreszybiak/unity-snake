using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;

public class Game : MonoBehaviour
{
    [SerializeField]
    Tilemap tilemap;

    [SerializeField]
    private Tile foodTile, floorTile, snakeTile;

    [SerializeField]
    private Snake snake; //USELESS?

    public Vector3Int foodPosition;

    private void Awake()
    {
        // Register to event
        snake.AteFood += OnSnakeAteFood;
        snake.ExecutedMove += OnSnakeExecutedMove;
        foodPosition = new Vector3Int(1, -2, 1);
    }

    private void OnDestroy()
    {
        snake.AteFood -= OnSnakeAteFood;
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

        foodPosition = freeTileCoordinates[Random.Range(0, freeTileCoordinates.Count)];        
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
        GenerateFood();
    }

    private void OnSnakeExecutedMove()
    {
        UpdateTilemap();
    }
}
