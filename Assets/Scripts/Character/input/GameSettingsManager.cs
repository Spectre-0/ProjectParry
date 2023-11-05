using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettingsManager : MonoBehaviour
{
    public static GameSettingsManager Instance;

    public float FOV { get; private set; } = 90f; // Default FOV value

    public delegate void SensitivityChangeDelegate(float newSensitivity);
    public event SensitivityChangeDelegate OnSensitivityChanged;

    public float MouseSensitivity { get; private set; } = 30f; // Default sensitivity


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Make this object persistent
        }
        else
        {
            Destroy(gameObject);
        }

        LoadSettings();
    }

    public void SetFOV(float value)
    {
        FOV = value;
        PlayerPrefs.SetFloat("PlayerFOV", value);
        // You might want to save PlayerPrefs here only if there are no other settings to change simultaneously
    }

    public void SetMouseSensitivity(float value)
    {
        MouseSensitivity = value;
        PlayerPrefs.SetFloat("MouseSensitivity", value);
        OnSensitivityChanged?.Invoke(value); // Fire the event
    }

    public void LoadSettings()
    {
        FOV = PlayerPrefs.GetFloat("PlayerFOV", 90f); // Load FOV, default to 90 if not set
        MouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 30f); // Load sensitivity, default to 30 if not set
    }

    private void OnDestroy()
    {
        PlayerPrefs.Save(); // Save the PlayerPrefs when this object is destroyed
    }
}