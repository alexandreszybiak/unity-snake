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
    const float MIN_MOVE_INTERVAL = .1f;
    const float INITIAL_MOVE_INTERVAL = .15f;
    const float ACCELERATION_INCREMENT = .005f;

    [SerializeField]
    Game gameManager;

    [SerializeField]
    Tilemap tilemap;

    [SerializeField] //Can it be replaced by a cleaner interface?
    Tile wallTile, floorTile, foodTile, bodyTile, tailTile, cornerTile, headOpenTile, headClosedTile;

    private float moveInterval;
    private float lastMoveTime;
    private Vector2Int autoMoveDirection;
    private bool moving;

    private int chancesLeft;

    private bool headOpen;

    private List<Vector2Int> parts;

    private void Awake()
    {
        //Init snake body
        headOpen = false;
        parts = new List<Vector2Int>();
        for (int i = 0; i > -3; i--)
        {
            parts.Add(new Vector2Int(i, -2));
        }

        //Init movement
        moveInterval = INITIAL_MOVE_INTERVAL;
        lastMoveTime = Time.time;
        autoMoveDirection = Vector2Int.right;
        moving = false;

        //
        chancesLeft = MAX_CHANCES;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(moving && lastMoveTime + moveInterval < Time.time)
        {
            Move(autoMoveDirection, false);
        }
    }

    public void DrawInTilemap()
    {
        
        Vector2Int diffWithPrevious = Vector2Int.zero;
        Vector2Int diffWithNext = Vector2Int.zero;
        TileBase t = null;
        for (int i = 0; i < parts.Count; i++)
        {
            float angle = 0;
            t = bodyTile;
            
            if(i < parts.Count - 1) diffWithNext = parts[i] - parts[i + 1];
            if (i > 0) diffWithPrevious = parts[i] - parts[i - 1];

            if (i == 0)
            {
                t = headOpen ? headOpenTile : headClosedTile;
                angle = Mathf.Atan2(diffWithNext.y, diffWithNext.x) * (180 / Mathf.PI);
            }
            else if (i == parts.Count - 1)
            {
                t = tailTile;
                angle = Mathf.Atan2(diffWithPrevious.y, diffWithPrevious.x) * (180 / Mathf.PI);
                
            }
            else
            {
                if(diffWithPrevious.x == diffWithNext.x)
                {
                    angle = 90;
                }
                else if(diffWithPrevious.y == diffWithNext.y)
                {
                    angle = 0;
                }
                else
                {
                    t = cornerTile;
                    Vector2Int addedDiff = diffWithPrevious + diffWithNext;
                    angle = Mathf.Atan2(addedDiff.y, addedDiff.x) * (180 / Mathf.PI) - 45.0f;
                }
            }

            Vector3Int cellCoord = new Vector3Int(parts[i].x, parts[i].y, 1);
            tilemap.SetTile(cellCoord, t);

            Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, angle), Vector3.one);
            tilemap.SetTransformMatrix(cellCoord, matrix);
        }
        headOpen = !headOpen;
    }

    void Move(Vector2Int moveDirection, bool intentional)
    {
        Vector2Int targetPos = parts[0] + moveDirection;
        bool collideWall = tilemap.GetTile(new Vector3Int(targetPos.x, targetPos.y, 0)) == wallTile;
        bool collideSelf = parts.Contains(targetPos);
        bool collideFood = tilemap.GetTile(new Vector3Int(targetPos.x, targetPos.y, 1)) == foodTile;

        if (collideWall || collideSelf)
        {
            if(intentional) return;

            if (chancesLeft == 0)
            {
                tilemap.color = Color.red;
                SceneManager.LoadScene("Title");
            }
            lastMoveTime = Time.time;
            chancesLeft--;
            return;
        }

        for (int i = parts.Count - 1; i > 0; i--)
        {
            parts[i] = parts[i - 1];
        }
        parts[0] += moveDirection;

        if (collideFood)
        {
            gameManager.GenerateFood();
            parts.Add(new Vector2Int(parts.Last().x, parts.Last().y));
            moveInterval = Mathf.Clamp(moveInterval - ACCELERATION_INCREMENT, MIN_MOVE_INTERVAL, INITIAL_MOVE_INTERVAL);
        }

        lastMoveTime = Time.time;
        autoMoveDirection = moveDirection;

        chancesLeft = MAX_CHANCES;

        gameManager.UpdateTilemap();
    }

    void OnMoveX(InputValue i)
    {
        if (moving == false) moving = true;

        var val = i.Get<float>();

        if (val == 0) return;

        if (autoMoveDirection.x != 0) return;
            
        Move(Vector2Int.FloorToInt(new Vector2(val, 0)), true);
    }

    void OnMoveY(InputValue i)
    {
        if (moving == false) moving = true;

        var val = i.Get<float>();

        if (val == 0) return;

        if (autoMoveDirection.y != 0) return;

        Move(Vector2Int.FloorToInt(new Vector2(0, val)), true);
    }
}
