using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Import the EventSystems namespace
using TMPro;

public class SensitivitySlider : MonoBehaviour
{
    AudioManager audioManager;
    public Slider sensitivitySlider;
    public TextMeshProUGUI sensitivityText;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private void Start()
    {
        sensitivitySlider.value = GameSettingsManager.Instance.MouseSensitivity;
        UpdateSensitivityText(sensitivitySlider.value); // Update the text at start

        sensitivitySlider.onValueChanged.AddListener(delegate { ValueChangeCheck(); }); // Listen for value changes

        // Add listener for OnPointerUp event
        EventTrigger trigger = sensitivitySlider.gameObject.AddComponent<EventTrigger>();
        var pointerUp = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        pointerUp.callback.AddListener((data) => { OnSensitivityChanged(sensitivitySlider.value); });
        trigger.triggers.Add(pointerUp);
    }

    // Method to update the sensitivity text
    private void UpdateSensitivityText(float value)
    {
        if (sensitivityText != null)
        {
            sensitivityText.text = $"{value.ToString("0.0")}"; // Display with 1 decimal point
        }
    }

    // Method called when the slider value changes but not used to play sound
    public void ValueChangeCheck()
    {
        GameSettingsManager.Instance.SetMouseSensitivity(sensitivitySlider.value);
        UpdateSensitivityText(sensitivitySlider.value); // Update the text when the value changes
    }

    // Method to play sound and update settings
    public void OnSensitivityChanged(float value)
    {
        audioManager.PlaySFX(audioManager.buttoinClickAudio);
        // The rest of the code is moved to ValueChangeCheck
    }
}
