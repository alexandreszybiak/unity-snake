using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class Snake : MonoBehaviour
{
    const int MAX_CHANCES = 1;

    [SerializeField]
    Game gameManager;

    [SerializeField]
    Tilemap tilemap;

    [SerializeField]
    Tile wallTile, floorTile, snakeTile, foodTile;

    private float moveInterval;
    private float lastMoveTime;
    private Vector2Int autoMoveDirection;

    private int chancesLeft;

    private List<Vector2Int> parts;
    void Start()
    {
        //Init snake body
        parts = new List<Vector2Int>();
        for(int i = 0; i < 5; i++)
        {
            parts.Add(new Vector2Int(-i, 0));
        }

        foreach(Vector2Int v in parts)
        {
            tilemap.SetTile(new Vector3Int(v.x, v.y, 0), snakeTile);
        }

        //Init movement
        moveInterval = .15f;
        lastMoveTime = Time.time;
        autoMoveDirection = Vector2Int.right;

        //
        chancesLeft = MAX_CHANCES;
    }

    // Update is called once per frame
    void Update()
    {
        if(lastMoveTime + moveInterval < Time.time)
        {
            Move(autoMoveDirection, false);
        }
    }

    void DrawInTilemap()
    {

    }

    void Move(Vector2Int moveDirection, bool intentional)
    {
        Vector2Int targetPos = parts[0] + moveDirection;
        TileBase t = tilemap.GetTile(new Vector3Int(targetPos.x, targetPos.y, 0));
        if (t == wallTile || t == snakeTile)
        {
            if(intentional) return;

            if (chancesLeft == 0)
            {
                tilemap.color = Color.red;
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            lastMoveTime = Time.time;
            chancesLeft--;
            return;
        }

        tilemap.SetTile(new Vector3Int(parts.Last().x, parts.Last().y), floorTile);

        for (int i = parts.Count - 1; i > 0; i--)
        {
            parts[i] = parts[i - 1];
            tilemap.SetTile(new Vector3Int(parts[i].x, parts[i].y), snakeTile);
        }
        parts[0] += moveDirection;
        tilemap.SetTile(new Vector3Int(parts[0].x, parts[0].y), snakeTile);

        if (t == foodTile)
        {
            gameManager.GenerateFood();
            parts.Add(new Vector2Int(parts.Last().x, parts.Last().y));
            tilemap.SetTile(new Vector3Int(parts.Last().x, parts.Last().y), snakeTile);
            moveInterval = Mathf.Clamp(moveInterval - .005f, .08f, .15f);
        }
        //tilemap.RefreshAllTiles();

        lastMoveTime = Time.time;
        autoMoveDirection = moveDirection;

        chancesLeft = MAX_CHANCES;
    }

    void OnMoveX(InputValue i)
    {
        var val = i.Get<float>();

        if (val == 0) return;

        if (autoMoveDirection.x != 0) return;
            
        Move(Vector2Int.FloorToInt(new Vector2(val, 0)), true);
    }

    void OnMoveY(InputValue i)
    {
        var val = i.Get<float>();

        if (val == 0) return;

        if (autoMoveDirection.y != 0) return;

        Move(Vector2Int.FloorToInt(new Vector2(0, val)), true);
    }
}
