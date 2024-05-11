using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    [SerializeField]
    GameObject gameManager, snake, scorePanel, title;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        gameManager.SetActive(true);
        snake.SetActive(true);
        //scorePanel.SetActive(true);

        title.SetActive(false);
        transform.parent.gameObject.SetActive(false);
    }
}
