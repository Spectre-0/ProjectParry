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
        //restasrt the  cuurent scene

        Application.LoadLevel(Application.loadedLevel);

        // unpause the game
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