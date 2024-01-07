using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Import the EventSystems namespace
using TMPro;

public class MusicSlider : MonoBehaviour
{
    AudioManager audioManager;
    public Slider musicSlider;
    public TextMeshProUGUI musicText;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    void Start()
    {
        musicSlider.value = GameSettingsManager.Instance.MusicVolume;
        UpdateMusicText(musicSlider.value); // Update the text at start

        musicSlider.onValueChanged.AddListener(delegate { ValueChangeCheck(); }); // Listen for value changes

        // Add listener for OnPointerUp event
        EventTrigger trigger = musicSlider.gameObject.AddComponent<EventTrigger>();
        var pointerUp = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        pointerUp.callback.AddListener((data) => { OnMusicSliderChanged(musicSlider.value); });
        trigger.triggers.Add(pointerUp);
    }

    // Method to update the music text
    private void UpdateMusicText(float value)
    {
        if (musicText != null)
        {
            musicText.text = $"{(value * 100).ToString("0")}";
        }
    }

    // Method called when the slider value changes but not used to play sound
    public void ValueChangeCheck()
    {
        GameSettingsManager.Instance.SetMusicVolume(musicSlider.value);
        audioManager.UpdateMusicVolume(musicSlider.value);
        UpdateMusicText(musicSlider.value); // Update the text when the value changes
    }

    // Method to play sound and update settings
    public void OnMusicSliderChanged(float value)
    {
        audioManager.PlaySFX(audioManager.buttoinClickAudio);
        // The rest of the code is moved to ValueChangeCheck
    }
}