using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;  

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuPanel;      // Assign the MainMenu panel from the inspector
    public GameObject optionsMenuPanel;   // Assign the OptionsMenu panel from the inspector
    public GameObject keyBindMenuPanel;   // Assign the KeyBindMenu panel from the inspector

    public GameObject levelSelectPanel;   // Assign the LevelSelect panel from the inspector

    public TextMeshProUGUI level1HighScoreText; // Assign your 'High Score Text' UI element in the inspector
    public TextMeshProUGUI level2HighScoreText; // Assign your 'High Score Text' UI element in the inspector
    public TextMeshProUGUI level3HighScoreText; // Assign your 'High Score Text' UI element in the inspector

    public GameObject level2LockImage; // Assign the Level 2 lock image in the inspector
    public GameObject level3LockImage; // Assign the Level 3 lock image in the inspector

    

    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private void Start()
    {
        ShowMainMenu();
        SetHighScoreText();
        UpdateLevelLocks();
    }

    public void PlayGame()
    {
        audioManager.PlaySFX(audioManager.buttoinClickAudio);
        LevelSelect();

    }

    // set the high score text for each level from the game settings manager

    public void SetHighScoreText()
    {
        level1HighScoreText.text = "High score: "+GameSettingsManager.Instance.GetHighScoreForLevel(1).ToString();
        level2HighScoreText.text = "High score: "+GameSettingsManager.Instance.GetHighScoreForLevel(2).ToString();
        level3HighScoreText.text = "High score:" +GameSettingsManager.Instance.GetHighScoreForLevel(3).ToString();
    }

    public void LevelSelect()
    {
        mainMenuPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
        optionsMenuPanel.SetActive(false);
        keyBindMenuPanel.SetActive(false);

    }

    void UpdateLevelLocks()
    {
        // Unlock Level 2 if Level 1 high score is greater than 0
        if (GameSettingsManager.Instance.GetHighScoreForLevel(1) > 0)
        {
            level2LockImage.SetActive(false);
        }
        else
        {
            level2LockImage.SetActive(true);
        }

        // Unlock Level 3 if Level 2 high score is greater than 0
        if (GameSettingsManager.Instance.GetHighScoreForLevel(2) > 0)
        {
            level3LockImage.SetActive(false);
        }
        else
        {
            level3LockImage.SetActive(true);
        }
    }

    public void resetHighScore()
    {
        GameSettingsManager.Instance.ResetHighScore();
        SetHighScoreText();
        UpdateLevelLocks(); // Update locks after resetting scores
    }

    public void Level1()
    {
        audioManager.PlaySFX(audioManager.buttoinClickAudio);
        LoadLevel(1);
    }

    public void Level2()
    {
        audioManager.PlaySFX(audioManager.buttoinClickAudio);
        LoadLevel(2);
    }

    public void Level3()
    {
        audioManager.PlaySFX(audioManager.buttoinClickAudio);
        LoadLevel(3);
    }

    // Common method to load levels
    private void LoadLevel(int levelIndex)
    {
        Time.timeScale = 1; // Ensure time scale is set to 1
        SceneManager.LoadScene(levelIndex); // Load the specified level
    }

    public void QuitGame()
    {
        audioManager.PlaySFX(audioManager.buttoinClickAudio);
        Debug.Log("Quit");
        Application.Quit();
        
    }

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        optionsMenuPanel.SetActive(false);
        keyBindMenuPanel.SetActive(false);
        levelSelectPanel.SetActive(false);
        audioManager.PlaySFX(audioManager.buttoinClickAudio);
    }

    public void ShowOptionsMenu()
    {
        mainMenuPanel.SetActive(false);
        optionsMenuPanel.SetActive(true);
        keyBindMenuPanel.SetActive(false);
        levelSelectPanel.SetActive(false);
        audioManager.PlaySFX(audioManager.buttoinClickAudio);
    }

    public void ShowKeyBindMenu()
    {
        mainMenuPanel.SetActive(false);
        optionsMenuPanel.SetActive(false);
        keyBindMenuPanel.SetActive(true);
        levelSelectPanel.SetActive(false);
        audioManager.PlaySFX(audioManager.buttoinClickAudio);
    }

    public void BackFromKeyBindsToOptions()
    {
        ShowOptionsMenu();
    }

    public void BackFromOptionsToMainMenu()
    {
        ShowMainMenu();
    }
}