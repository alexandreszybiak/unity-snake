using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapUpdater : MonoBehaviour
{
    [SerializeField]
    Snake snake;

    [SerializeField]
    Game gameManager;

    [SerializeField]
    Tile wallTile, floorTile, snakeTile, foodTile;

    private Tilemap tilemap;

    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        Clear();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Clear()
    {
        BoundsInt area = new BoundsInt(1, 1, 0, tilemap.cellBounds.max.x - 1, tilemap.cellBounds.max.y - 1, 0);
        TileBase[] tileArray = new TileBase[area.size.x * area.size.y];
        for (int index = 0; index < tileArray.Length; index++)
        {
            tileArray[index] = foodTile;
        }
        tilemap.SetTilesBlock(area, tileArray);
    }
}
