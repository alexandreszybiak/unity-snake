using System;
using System.Collections.Generic;
using UnityEngine;

public class ScorePanel : MonoBehaviour
{
    [SerializeField]
    private GameObject counterOnes, counterTens;

    [SerializeField]
    private List<Sprite> numberFont;

    [SerializeField]
    private Snake snake;

    private int count;

    public event Action madeNewHighscore;
    private void Awake()
    {
        count = 0;

        if (snake != null)
        {
            snake.AteFood += OnSnakeAteFood;
            snake.FinishedGameOverSequence += OnSnakeFinishedGameOverSequence;
            snake.Died += OnSnakeDied;
        }

    }
    private void OnDestroy()
    {
        if (snake != null)
        {
            snake.AteFood -= OnSnakeAteFood;
            snake.FinishedGameOverSequence -= OnSnakeFinishedGameOverSequence;
            snake.Died -= OnSnakeDied;
        }
    }
    void Start()
    {
        SetScore(0);
    }

    private void SetScore(int value = 0)
    {
        counterTens.GetComponent<SpriteRenderer>().sprite = numberFont[value / 10];
        counterOnes.GetComponent<SpriteRenderer>().sprite = numberFont[value % 10];
    }

    private void OnSnakeAteFood()
    {
        count++;
        SetScore(count);
    }

    private void OnSnakeFinishedGameOverSequence()
    {
        ResetCounter();
    }

    private void ResetCounter()
    {
        SetScore(0);
        count = 0;
    }

    private void OnSnakeDied()
    {
        if (count < PlayerPrefs.GetInt("Score", 10)) return;

        PlayerPrefs.SetInt("Score", count);

        madeNewHighscore?.Invoke();
    }
}
