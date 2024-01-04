using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuPanel;      // Assign the MainMenu panel from the inspector
    public GameObject optionsMenuPanel;   // Assign the OptionsMenu panel from the inspector
    public GameObject keyBindMenuPanel;   // Assign the KeyBindMenu panel from the inspector

    public GameObject levelSelectPanel;   // Assign the LevelSelect panel from the inspector

    private void Start()
    {
        ShowMainMenu();
    }

    public void PlayGame()
    {
        LevelSelect();

    }

    public void LevelSelect()
    {
        mainMenuPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
        optionsMenuPanel.SetActive(false);
        keyBindMenuPanel.SetActive(false);

    }

    public void Level1()
    {
        LoadLevel(1);
    }

    public void Level2()
    {
        LoadLevel(2);
    }

    public void Level3()
    {
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
        Debug.Log("Quit");
        Application.Quit();
    }

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        optionsMenuPanel.SetActive(false);
        keyBindMenuPanel.SetActive(false);
        levelSelectPanel.SetActive(false);
    }

    public void ShowOptionsMenu()
    {
        mainMenuPanel.SetActive(false);
        optionsMenuPanel.SetActive(true);
        keyBindMenuPanel.SetActive(false);
        levelSelectPanel.SetActive(false);
    }

    public void ShowKeyBindMenu()
    {
        mainMenuPanel.SetActive(false);
        optionsMenuPanel.SetActive(false);
        keyBindMenuPanel.SetActive(true);
        levelSelectPanel.SetActive(false);
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