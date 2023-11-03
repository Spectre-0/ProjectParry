using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Debug.Log("Restarting");
        // restart the level without scene manager
        Application.LoadLevel(Application.loadedLevel);

        
    }

    public void QuitGame()
    {
        Application.Quit();
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