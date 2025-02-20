using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class Snake : MonoBehaviour
{
    public bool paused;

    const int MAX_CHANCES = 1;
    const float MIN_MOVE_INTERVAL = .125f;
    const float INITIAL_MOVE_INTERVAL = .15f; // .15f
    const float ACCELERATION_INCREMENT = .015f; //.015f

    [SerializeField]
    Tilemap tilemap;

    [SerializeField] //Can it be replaced by a cleaner interface?
    Tile wallTile, floorTile, foodTile, bodyTile, brokenBodyTile, tailTile, brokenTailTile, cornerTile, headOpenTile, headClosedTile, headHitWallTile, headKissClosedTile, headKissOpenTile;

    private float internalTime;
    private float moveInterval;
    private float lastMoveTime;
    private Vector2Int autoMoveDirection;
    private bool moving;
    private Vector2Int savedLastIntentionalDirection;

    private int chancesLeft;

    private Tile currentHeadTile;
    private Tile currentTailTile;
    private Tile currentBodyTile;

    public List<Vector2Int> parts;

    // Events
    public event Action AteFood;
    public event Action ExecutedMove;
    public event Action FinishedGameOverSequence;
    public event Action Died;
    public event Action LoosePart;
    public event Action ChangeDirection;
    public event Action GotControl;
    public event Action AutoMoved;
    public event Action StartPlaying;
    public event Action Created;

    private void Awake()
    {
        //Init snake body
        currentHeadTile = headClosedTile;
        currentTailTile = tailTile;
        currentBodyTile = bodyTile;

        parts = new List<Vector2Int>();
        for (int i = 0; i > -3; i--)
        {
            parts.Add(new Vector2Int(i, -2));
        }
        Created?.Invoke();

        //Init movement
        internalTime = 0.0f;
        moveInterval = INITIAL_MOVE_INTERVAL;
        lastMoveTime = internalTime;
        autoMoveDirection = Vector2Int.right;
        moving = false;
        savedLastIntentionalDirection = Vector2Int.zero;

        //
        chancesLeft = MAX_CHANCES;

        // Sound
        SfxManager sfxManager = FindAnyObjectByType<SfxManager>();

        if (sfxManager == null) return;

        AteFood += sfxManager.OnSnakeAteFood;
        Died += sfxManager.OnSnakeDied;
        LoosePart += sfxManager.OnSnakeLoosePart;
        ChangeDirection += sfxManager.OnSnakeChangeDirection;
        GotControl += sfxManager.OnSnakeGotControl;
        AutoMoved += sfxManager.OnSnakeAutoMove;

        // Music
        MusicManager musicManager = FindAnyObjectByType<MusicManager>();

        if (musicManager == null) return;

        StartPlaying += musicManager.OnGameStart;
        Died += musicManager.OnGameStop;
    }
    void Start()
    {
        GotControl?.Invoke();
    }

    private void OnDestroy()
    {
        SfxManager sfxManager = FindAnyObjectByType<SfxManager>();

        if (sfxManager == null) return;

        AteFood -= sfxManager.OnSnakeAteFood;
        Died -= sfxManager.OnSnakeDied;
        LoosePart -= sfxManager.OnSnakeLoosePart;
        ChangeDirection -= sfxManager.OnSnakeChangeDirection;
        GotControl -= sfxManager.OnSnakeGotControl;
        AutoMoved -= sfxManager.OnSnakeAutoMove;

        // Music
        MusicManager musicManager = FindAnyObjectByType<MusicManager>();

        if (musicManager == null) return;

        StartPlaying -= musicManager.OnGameStart;
        Died -= musicManager.OnGameStop;
    }

    // Update is called once per frame
    void Update()
    {
        if (paused) return;

        internalTime += Time.deltaTime;
        if(moving && lastMoveTime + moveInterval < internalTime)
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
            t = currentBodyTile;
            
            if(i < parts.Count - 1) diffWithNext = parts[i] - parts[i + 1];
            if (i > 0) diffWithPrevious = parts[i] - parts[i - 1];

            if (i == 0)
            {
                t = currentHeadTile;
                angle = Mathf.Atan2(diffWithNext.y, diffWithNext.x) * (180 / Mathf.PI);
            }
            else if (i == parts.Count - 1)
            {
                t = currentTailTile;
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
    }

    void Move(Vector2Int wantedDirection, bool intentional, bool isSavedMove = false)
    {
        if (!moving) return;

        Vector2Int moveDirection = wantedDirection;  
        Vector2Int targetPos = parts[0] + moveDirection;
        bool collideWall = tilemap.GetTile(new Vector3Int(targetPos.x, targetPos.y, 0)) == wallTile;
        bool collideSelf = parts.Contains(targetPos);
        bool collideFood = tilemap.GetTile(new Vector3Int(targetPos.x, targetPos.y, 1)) == foodTile;

        if (collideWall || collideSelf)
        {
            
            if (intentional)
            {
                if (isSavedMove == false)
                {
                    savedLastIntentionalDirection = wantedDirection;
                }
                return;
            }

            if (chancesLeft == 0)
            {
                Die();
                return;
            }
            lastMoveTime = internalTime;
            chancesLeft--;
            currentHeadTile = currentHeadTile == headOpenTile ? headKissOpenTile : headKissClosedTile;
            ExecutedMove?.Invoke();
            return;
        }

        if (collideFood)
        {
            parts.Add(parts.Last());
            moveInterval = Mathf.Clamp(moveInterval - ACCELERATION_INCREMENT, MIN_MOVE_INTERVAL, INITIAL_MOVE_INTERVAL);
        }

        for (int i = parts.Count - 1; i > 0; i--)
        {
            parts[i] = parts[i - 1];
        }
        parts[0] += moveDirection;

        //
        autoMoveDirection = moveDirection;

        // Saved move
        if (intentional)
        {
            ChangeDirection?.Invoke();
            savedLastIntentionalDirection = Vector2Int.zero;
        }
        else
        {
            AutoMoved?.Invoke();
        }
        if (savedLastIntentionalDirection != Vector2Int.zero)
        {
            Move(savedLastIntentionalDirection, true, true);
            savedLastIntentionalDirection = Vector2Int.zero;
        }

        // Send message that I ate food
        if (collideFood) AteFood?.Invoke();

        lastMoveTime = internalTime;
        
        chancesLeft = MAX_CHANCES;

        // Change head
        if (currentHeadTile == headKissOpenTile) currentHeadTile = headOpenTile;
        else if (currentHeadTile == headKissClosedTile) currentHeadTile = headClosedTile;
        else currentHeadTile = currentHeadTile == headOpenTile ? headClosedTile : headOpenTile;

        // Message that I finished moving
        if (isSavedMove == false) ExecutedMove?.Invoke();

        
    }

    void OnMoveX(InputValue i)
    {
        if (moving == false)
        {
            StartPlaying?.Invoke();
            moving = true;
        }

        var val = i.Get<float>();

        if (val == 0) return;

        if (autoMoveDirection.x != 0) return;
            
        Move(Vector2Int.FloorToInt(new Vector2(val, 0)), true);
    }

    void OnMoveY(InputValue i)
    {
        if (moving == false)
        {
            StartPlaying?.Invoke();
            moving = true;
        }

        var val = i.Get<float>();

        if (val == 0) return;

        if (autoMoveDirection.y != 0) return;

        Move(Vector2Int.FloorToInt(new Vector2(0, val)), true);
    }

    private IEnumerator DeleteTail()
    {
        float time = Mathf.Clamp(0.8f / (parts.Count - 4), .04f, .07f);
        yield return new WaitForSeconds(0.5f);
        while(parts.Count > 3)
        {
            parts.RemoveAt(parts.Count - 1);
            ExecutedMove?.Invoke();
            LoosePart?.Invoke();
            yield return new WaitForSeconds(time);
        }
        yield return new WaitForSeconds(0.5f);
        GetComponent<PlayerInput>().enabled = true;
        currentHeadTile = headClosedTile;
        currentBodyTile = bodyTile;
        currentTailTile = tailTile;
        moveInterval = INITIAL_MOVE_INTERVAL;
        FinishedGameOverSequence?.Invoke();
        GotControl?.Invoke();
    }

    private void Die()
    {
        currentHeadTile = headHitWallTile;
        currentBodyTile = brokenBodyTile;
        currentTailTile = brokenTailTile;
        Died?.Invoke();
        //tilemap.color = Color.red;
        moving = false;
        GetComponent<PlayerInput>().enabled = false;
        StartCoroutine(DeleteTail());
    }
}
