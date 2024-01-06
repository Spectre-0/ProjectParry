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


    public int level1HighScore = 0;
    public int level2HighScore = 0;
    public int level3HighScore = 0;


    public float SFXVolume { get; private set; } = 0.5f; // Default value for SFX Volume
    public float MusicVolume { get; private set; } = 0.5f; // Default value for Music Volume


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

    public void SetLevel1HighScore(int value)
    {
        level1HighScore = value;
        PlayerPrefs.SetInt("Level1HighScore", value);
    }

    public void SetLevel2HighScore(int value)
    {
        level2HighScore = value;
        PlayerPrefs.SetInt("Level2HighScore", value);
    }

    public void SetLevel3HighScore(int value)
    {
        level3HighScore = value;
        PlayerPrefs.SetInt("Level3HighScore", value);
    }


    public int GetHighScoreForLevel(int levelIndex)
    {
        return PlayerPrefs.GetInt($"Level{levelIndex}HighScore", 0);
    }

    // Method to set the high score for a specific level
    public void SetHighScoreForLevel(int levelIndex, int value)
    {
        PlayerPrefs.SetInt($"Level{levelIndex}HighScore", value);

        // Update the high score in the instance as well
        switch (levelIndex)
        {
            case 1:
                level1HighScore = value;
                break;
            case 2:
                level2HighScore = value;
                break;
            case 3:
                level3HighScore = value;
                break;
            default:
                Debug.LogError("Invalid level index");
                break;
        }

        PlayerPrefs.Save(); // Save the change
    }

    // reset high score for all levels

    public void ResetHighScore()
    {
        PlayerPrefs.DeleteKey("Level1HighScore");
        PlayerPrefs.DeleteKey("Level2HighScore");
        PlayerPrefs.DeleteKey("Level3HighScore");
        level1HighScore = 0;
        level2HighScore = 0;
        level3HighScore = 0;
    }


    public void SetSFXVolume(float value)
    {
        SFXVolume = value;
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    public void SetMusicVolume(float value)
    {
        MusicVolume = value;
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void LoadSettings()
    {
        FOV = PlayerPrefs.GetFloat("PlayerFOV", 90f); // Load FOV, default to 90 if not set
        MouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 30f); // Load sensitivity, default to 30 if not set

        SFXVolume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
        MusicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        level1HighScore = PlayerPrefs.GetInt("Level1HighScore", 0);
        level2HighScore = PlayerPrefs.GetInt("Level2HighScore", 0);
        level3HighScore = PlayerPrefs.GetInt("Level3HighScore", 0);
        
    }
    

    private void OnDestroy()
    {
        PlayerPrefs.Save(); // Save the PlayerPrefs when this object is destroyed
    }
}