using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMenu : MonoBehaviour
{
    public GameObject playerMenuPanel;      // Assign the MainMenu panel from the inspector
    public GameObject optionsMenuPanel;   // Assign the OptionsMenu panel from the inspector
    public GameObject keyBindMenuPanel;   // Assign the KeyBindMenu panel from the inspector

    private void Start()
    {
        ShowPlayerMenu();
    }
    public void Restart()
    {
        // Restart the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        // Unpause the game by calling the UnpauseGame method in PlayerMotor
        if (playerMenuPanel.activeSelf)
        {
            PlayerMotor playerMotor = FindObjectOfType<PlayerMotor>();
            if (playerMotor)
            {
                playerMotor.UnpauseGame();
            }
        }
    }

    public void nextLevel()
    {
        // Go to the next level
        int nextLevelIndex = Application.loadedLevel + 1;
        if (nextLevelIndex >= Application.levelCount)
        {
            nextLevelIndex = 0; // Go to the first level if index is out of range
        }

        Application.LoadLevel(nextLevelIndex);

        // Unpause the game
        Time.timeScale = 1;
    }
    

    

    public void ExitToMainMenu()
    {
        Debug.Log("Exiting to main menu");
        // unpause the game
        Time.timeScale = 1;
        
        // load the main menu scene
        Application.LoadLevel(0);
    }

    public void ShowPlayerMenu()
    {
        playerMenuPanel.SetActive(true);
        optionsMenuPanel.SetActive(false);
        keyBindMenuPanel.SetActive(false);
    }

    public void ShowOptionsMenu()
    {
        playerMenuPanel.SetActive(false);
        optionsMenuPanel.SetActive(true);
        keyBindMenuPanel.SetActive(false);
    }

    public void ShowKeyBindMenu()
    {
        playerMenuPanel.SetActive(false);
        optionsMenuPanel.SetActive(false);
        keyBindMenuPanel.SetActive(true);
    }

    public void BackFromKeyBindsToOptions()
    {
        ShowOptionsMenu();
    }

    public void BackFromOptionsToMainMenu()
    {
        ShowPlayerMenu();
    }
}