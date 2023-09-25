using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HighscorePanel : MonoBehaviour
{
    [SerializeField]
    private GameObject counterOnes, counterTens;

    [SerializeField]
    private List<Sprite> numberFont;

    [SerializeField]
    private ScorePanel scorePanel;

    private void Awake()
    {
        if (scorePanel != null)
        {
            scorePanel.madeNewHighscore += OnNewHighscore;
        }
    }

    private void OnDestroy()
    {
        if (scorePanel != null)
        {
            scorePanel.madeNewHighscore -= OnNewHighscore;
        }
    }
    void Start()
    {
        SetValue(PlayerPrefs.GetInt("Score", 9));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetValue(int value = 0)
    {
        counterTens.GetComponent<SpriteRenderer>().sprite = numberFont[value / 10];
        counterOnes.GetComponent<SpriteRenderer>().sprite = numberFont[value % 10];
    }

    private void OnNewHighscore()
    {
        SetValue(PlayerPrefs.GetInt("Score", 0));
        
    }
}
