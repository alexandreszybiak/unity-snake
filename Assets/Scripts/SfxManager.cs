using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxManager : MonoBehaviour
{
    AudioSource audioSource;

    [SerializeField]
    AudioClip snakeReadySfx, eatFruitSfx, crashSfx, loosePartSfx, changeDirectionSfx, enterPauseSfx, exitPauseSfx, autoMoveSfx;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSnakeAteFood()
    {
        audioSource.PlayOneShot(eatFruitSfx, 1.0f);
    }

    public void OnSnakeDied()
    {
        audioSource.PlayOneShot(crashSfx);
    }

    public void OnSnakeLoosePart()
    {
        audioSource.PlayOneShot(loosePartSfx, 0.5f);
    }

    public void OnSnakeChangeDirection()
    {
        audioSource.PlayOneShot(changeDirectionSfx, 0.50f);
    }

    public void OnSnakeGotControl()
    {
        audioSource.PlayOneShot(snakeReadySfx);
    }

    public void OnEnterPause()
    {
        audioSource.PlayOneShot(enterPauseSfx);
    }

    public void OnExitPause()
    {
        audioSource.PlayOneShot(exitPauseSfx);
    }

    public void OnSnakeAutoMove()
    {
        //audioSource.PlayOneShot(autoMoveSfx, 0.05f);
    }

    public void SetMute(bool mute)
    {
        audioSource.mute = !mute;
    }
}
