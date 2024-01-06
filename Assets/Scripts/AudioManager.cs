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
    public AudioClip BossAttackAudio;
    public AudioClip BossDeathAudio;

    public AudioClip BossHitPlayerAudio;
    public AudioClip BossgetHitAudio;
    public AudioClip BossWalkAudio;

    public AudioClip mobAttackAudio;
    public AudioClip mobDeathAudio;
    public AudioClip mobHitPlayerAudio;
    public AudioClip mobgetHitAudio;
    public AudioClip mobWalkAudio;
    public AudioClip playerAttackAudio;
    

    public AudioClip playerDeathAudio;

    public AudioClip playerWalkAudio;

    public AudioClip playerJumpAudio;


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






