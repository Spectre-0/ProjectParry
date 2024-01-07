using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Import the EventSystems namespace
using TMPro;

public class SFXSlider : MonoBehaviour
{
    AudioManager audioManager;
    public Slider sfxSlider;
    public TextMeshProUGUI sfxText;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    void Start()
    {
        sfxSlider.value = GameSettingsManager.Instance.SFXVolume;
        UpdateSFXText(sfxSlider.value); // Update the text at start

        sfxSlider.onValueChanged.AddListener(delegate { ValueChangeCheck(); }); // Listen for value changes

        // Add listener for OnPointerUp event
        EventTrigger trigger = sfxSlider.gameObject.AddComponent<EventTrigger>();
        var pointerUp = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        pointerUp.callback.AddListener((data) => { OnSFXSliderChanged(sfxSlider.value); });
        trigger.triggers.Add(pointerUp);
    }

    // Method to update the SFX text
    private void UpdateSFXText(float value)
    {
        if (sfxText != null)
        {
            sfxText.text = $"{(value * 100).ToString("0")}";
        }
    }

    // Method called when the slider value changes but not used to play sound
    public void ValueChangeCheck()
    {
        GameSettingsManager.Instance.SetSFXVolume(sfxSlider.value);
        audioManager.UpdateSFXVolume(sfxSlider.value);
        UpdateSFXText(sfxSlider.value); // Update the text when the value changes
    }


    // Method to play sound and update settings
    public void OnSFXSliderChanged(float value)
    {
        audioManager.PlaySFX(audioManager.buttoinClickAudio);
        // The rest of the code is moved to ValueChangeCheck
    }
}