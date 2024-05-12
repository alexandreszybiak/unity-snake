using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    [SerializeField]
    private Snake snake;

    [SerializeField]
    private int initialHighest;

    [SerializeField]
    private Transform flag;

    private int highestScore;

    private SpriteRenderer spriteRenderer;

    private const float unit = 0.125f;

    private void Awake()
    {
        if (snake != null)
        {
            snake.AteFood += OnSnakeAteFood;
            snake.FinishedGameOverSequence += OnSnakeFinishedGameOverSequence;
            snake.Died += OnSnakeDied;
            snake.LoosePart += OnSnakeLostPart;
            snake.Created += OnSnakeCreated;
        }
    }
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        highestScore = initialHighest;

        flag.Translate(Vector3.right * unit * highestScore);

        //spriteRenderer.size += Vector2.right * unit * 2.0f;
    }

    private void OnSnakeCreated()
    {
        spriteRenderer.size = new Vector2(unit * snake.parts.Count, 0.1875f);
    }
    // Update is called once per frame
    void Update()
    {
        //spriteRenderer.size = new Vector2(0.125f * snake.parts.Count, 0.1875f);
    }
    private void OnSnakeAteFood()
    {
        spriteRenderer.size += Vector2.right * unit;

        if (snake.parts.Count > highestScore)
        {
            highestScore = snake.parts.Count;
            flag.localPosition = Vector3.right * unit * highestScore;
        }
    }

    private void OnSnakeFinishedGameOverSequence()
    {
        
    }

    private void OnSnakeDied()
    {
        
    }

    private void OnSnakeLostPart()
    {
        spriteRenderer.size += Vector2.left * unit;
    }
}
