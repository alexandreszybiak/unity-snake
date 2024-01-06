using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxManager : MonoBehaviour
{
    AudioSource audioSource;

    [SerializeField]
    AudioClip snakeReadySfx, eatFruitSfx, crashSfx, loosePartSfx, changeDirectionSfx;

    [SerializeField]
    private Snake snake; //USELESS?

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSnakeAteFood()
    {
        audioSource.PlayOneShot(eatFruitSfx);
    }

    public void OnSnakeDied()
    {
        audioSource.PlayOneShot(crashSfx);
    }

    public void OnSnakeLoosePart()
    {
        audioSource.PlayOneShot(loosePartSfx);
    }

    public void OnSnakeChangeDirection()
    {
        audioSource.PlayOneShot(changeDirectionSfx, 0.25f);
    }

    public void OnSnakeGotControl()
    {
        audioSource.PlayOneShot(snakeReadySfx);
    }
}
