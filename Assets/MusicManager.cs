using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    AudioSource audioSource;

    [SerializeField]
    AudioClip gameMusic, titleMusic, idleMusic;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void SetMute(bool mute)
    {
        audioSource.mute = !mute;
    }

    public void OnGameStart()
    {
        audioSource.Play();
    }

    public void OnGameStop()
    {
        audioSource.Stop();
    }
    public void OnEnterPause()
    {
        audioSource.Pause();
    }

    public void OnExitPause()
    {
        audioSource.UnPause();
    }
}
