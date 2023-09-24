using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.U2D;
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

    private void Awake()
    {
        count = 0;

        if(snake != null) snake.AteFood += OnSnakeAteFood;

    }
    private void OnDestroy()
    {
        if (snake != null) snake.AteFood -= OnSnakeAteFood;
    }
    void Start()
    {
        SetScore(0);
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
