using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class SFXSlider : MonoBehaviour
{

    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    public Slider sfxSlider;

    public TextMeshProUGUI sfxText;

    void Start()
    {
        sfxSlider.value = GameSettingsManager.Instance.SFXVolume;
        OnSFXSliderChanged(sfxSlider.value);
        sfxSlider.onValueChanged.AddListener(OnSFXSliderChanged);
        
    }

    public void OnSFXSliderChanged(float value)
    {
        GameSettingsManager.Instance.SetSFXVolume(value);
        audioManager.UpdateSFXVolume(value);
        if (sfxText != null)
        {
            sfxText.text = $"{(value*100).ToString("0")}";
        }
    }



}
