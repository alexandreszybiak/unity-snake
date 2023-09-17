using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class Snake : MonoBehaviour
{
    [SerializeField]
    Game gameManager;

    [SerializeField]
    Tilemap tilemap;

    [SerializeField]
    Tile wallTile, floorTile, snakeTile, foodTile;

    private float moveInterval;
    private float lastMoveTime;
    private Vector2Int autoMoveDirection;

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
    }

    // Update is called once per frame
    void Update()
    {
        if(lastMoveTime + moveInterval < Time.time)
        {
            Move(autoMoveDirection);
        }
    }

    void DrawInTilemap()
    {

    }

    void Move(Vector2Int moveDirection)
    {
        Vector2Int targetPos = parts[0] + moveDirection;
        TileBase t = tilemap.GetTile(new Vector3Int(targetPos.x, targetPos.y, 0));
        if (t == wallTile) return;

        if (t == foodTile) gameManager.GenerateFood();

        tilemap.SetTile(new Vector3Int(parts.Last().x, parts.Last().y), floorTile);        
        for (int i = parts.Count - 1; i > 0; i--)
        {
            parts[i] = parts[i - 1];
            tilemap.SetTile(new Vector3Int(parts[i].x, parts[i].y), snakeTile);
        }
        parts[0] += moveDirection;
        tilemap.SetTile(new Vector3Int(parts[0].x, parts[0].y), snakeTile);
        //tilemap.RefreshAllTiles();

        lastMoveTime = Time.time;
        autoMoveDirection = moveDirection;
    }

    void OnMoveX(InputValue i)
    {
        var val = i.Get<float>();

        if (val == 0) return;

        if (autoMoveDirection.x != 0) return;
            
        Move(Vector2Int.FloorToInt(new Vector2(val, 0)));
    }

    void OnMoveY(InputValue i)
    {
        var val = i.Get<float>();

        if (val == 0) return;

        if (autoMoveDirection.y != 0) return;

        Move(Vector2Int.FloorToInt(new Vector2(0, val)));
    }
}
