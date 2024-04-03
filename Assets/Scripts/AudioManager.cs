using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("----- Audio Source -----")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSouce;

    [Header("----- Audio Clip -----")]
    public AudioClip backgroundMusic;
    public AudioClip battleMusic;
    public AudioClip playerDeathSound;
    public AudioClip enemyDeathSound;
    public AudioClip coinCollection;
    public AudioClip powerupSound;
    public AudioClip powerdownSound;


    //Plays the BG music on start up
    private void Start()
    {
        musicSource.clip = backgroundMusic;
        musicSource.Play();
    }

    //Public method that will play the SFX that is fed into it
    public void PlaySFX(AudioClip Clip)
    {
        SFXSouce.PlayOneShot(Clip);
    }

    public void ChangeBGMusic(AudioClip Clip)
    {
        musicSource.clip = (Clip);
        musicSource.Play();
    }
}
