using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Import the EventSystems namespace
using TMPro;

public class FOVSlider : MonoBehaviour
{
    AudioManager audioManager;
    public Slider fovSlider;
    public TextMeshProUGUI fovText;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private void Start()
    {
        fovSlider.value = GameSettingsManager.Instance.FOV;
        UpdateFOVText(fovSlider.value); // Update the text at start
        ValueChangeCheck();

        fovSlider.onValueChanged.AddListener(delegate { ValueChangeCheck(); }); // Listen for value changes

        // Add listener for OnPointerUp event
        EventTrigger trigger = fovSlider.gameObject.AddComponent<EventTrigger>();
        var pointerUp = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        pointerUp.callback.AddListener((data) => { OnFOVSliderChanged(fovSlider.value); });
        trigger.triggers.Add(pointerUp);
    }

    // Method to update the FOV text
    private void UpdateFOVText(float value)
    {
        if (fovText != null)
        {
            fovText.text = $"{value.ToString("0")}";
        }
    }

    // Method called when the slider value changes but not used to play sound
    public void ValueChangeCheck()
    {
        GameSettingsManager.Instance.SetFOV(fovSlider.value);

        Camera playerCamera = Camera.main; // Assuming you want to change the main camera's FOV
        if (playerCamera != null)
        {
            playerCamera.fieldOfView = fovSlider.value;
        }
        UpdateFOVText(fovSlider.value); // Update the text when the value changes
    }

    // Method to play sound and update settings
    public void OnFOVSliderChanged(float value)
    {
        audioManager.PlaySFX(audioManager.buttoinClickAudio);
        // The rest of the code is moved to ValueChangeCheck
    }
}
