using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SensitivitySlider : MonoBehaviour
{
    public Slider sensitivitySlider;
    public TextMeshProUGUI sensitivityText;

    private void Start()
    {
        // Set the initial slider value and text from the GameSettingsManager
        sensitivitySlider.value = GameSettingsManager.Instance.MouseSensitivity;
        OnSensitivityChanged(sensitivitySlider.value); // Update the text display
        sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged); // Listen for changes
    }

    public void OnSensitivityChanged(float value)
    {
        // Update the setting in GameSettingsManager
        GameSettingsManager.Instance.SetMouseSensitivity(value);

        // Update the display text with the new value
        if (sensitivityText != null)
        {
            sensitivityText.text = $"Sensitivity: {value.ToString("0.0")}"; // Display with 1 decimal point
        }
    }
}
