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
    Tile foodTile, floorTile, snakeTile;

    [SerializeField]
    Snake snake; //USELESS?

    private Vector2Int foodPosition;
    void Start()
    {
        foodPosition = new Vector2Int(Random.Range(-7, 6), Random.Range(-6, 7));
        tilemap.SetTile(new Vector3Int(foodPosition.x, foodPosition.y, 0), foodTile);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateFood()
    {
        var freeTileCoordinates = new List<Vector3Int>();
        for(int x = tilemap.cellBounds.min.x; x < tilemap.cellBounds.max.x; x++)
        {
            for(int y = tilemap.cellBounds.min.y; y < tilemap.cellBounds.max.y; y++)
            {
                Vector3Int coord = new Vector3Int(x, y, 0);
                TileBase t = tilemap.GetTile(coord);
                if (t == floorTile) freeTileCoordinates.Add(coord);
            }
        }

        Vector3Int randomTileCoordinate = freeTileCoordinates[Random.Range(0, freeTileCoordinates.Count)];

        tilemap.SetTile(randomTileCoordinate, foodTile);
    }

    void OnRegenerateFood()
    {
        GenerateFood();
    }
}
