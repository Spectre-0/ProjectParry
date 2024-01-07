using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AudioManager : MonoBehaviour
{

    [Header("----------Audio Sources----------")]
    [SerializeField]  AudioSource musicSource;
    [SerializeField]  AudioSource SFXSource;

    [Header("----------Audio Clips----------")]
    public AudioClip backgroundMusic; 

    public AudioClip mobAttackAudio;
    
    public AudioClip playerAttackAudio;
    

    public AudioClip playerWalkAudio;


    public AudioClip buttoinClickAudio;

    public AudioClip playerGetHitAudio1;
    public AudioClip playerGetHitAudio2;

   


     private void Start()
    {
        // Set the audio sources to the saved volume levels
        UpdateMusicVolume(GameSettingsManager.Instance.MusicVolume);
        UpdateSFXVolume(GameSettingsManager.Instance.SFXVolume);

        musicSource.clip = backgroundMusic;
        musicSource.loop = true; // Set loop to true
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    // Call these methods when the slider values change
    public void UpdateMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void UpdateSFXVolume(float volume)
    {
        SFXSource.volume = volume;
    }
}






