using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class MusicSlider : MonoBehaviour
{


    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    public Slider musicSlider;

    public TextMeshProUGUI musicText;

    // Start is called before the first frame update
    void Start()
    {
        musicSlider.value = GameSettingsManager.Instance.MusicVolume;
        OnMusicSliderChanged(musicSlider.value);
        musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
        
    }

    public void OnMusicSliderChanged(float value)
    {
        GameSettingsManager.Instance.SetMusicVolume(value);
        audioManager.UpdateMusicVolume(value);
        if (musicText != null)
        {
            musicText.text = $"{(value*100).ToString("0")}";
        }
    }


}
