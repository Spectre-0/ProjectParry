using UnityEngine;
using UnityEngine.SceneManagement; // Needed for getting scene information
using UnityEngine.UI;
using TMPro;

public class BossEnemy : MonoBehaviour
{
    public GameObject WinMenuUI; // Assign your 'Win Menu' UI Panel in the inspector
    public TextMeshProUGUI score; // Assign your 'Score Text' UI element in the inspector

    public TextMeshProUGUI scoreText; // Assign your 'Score Text' UI element in the inspector
    public TextMeshProUGUI highScoreText; // Assign your 'High Score Text' UI element in the inspector

    public GameObject player; // Reference to the player game object
    public PlayerMotor playerMotor; // Reference to the PlayerMotor script to access the player's score

    private void OnDestroy()
    {
        // Stop the game
        Time.timeScale = 0;
        // Show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // Activate the 'Win Menu' UI panel
        WinMenuUI.SetActive(true);

        // disable ecape key while in win menu
    
        // Get the current score from the player
        int currentScore = playerMotor.GetScore();
        // Update the score display
        score.text = currentScore.ToString();

        // Get the current scene index
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Check if the current score is greater than the high score for the level
        if (GameSettingsManager.Instance.GetHighScoreForLevel(sceneIndex) < currentScore)
        {
            // Update the high score for the level
            GameSettingsManager.Instance.SetHighScoreForLevel(sceneIndex, currentScore);
            highScoreText.gameObject.SetActive(true);
            scoreText.gameObject.SetActive(false); // Hide the regular score text
        }
        else
        {
            highScoreText.gameObject.SetActive(false);
            scoreText.gameObject.SetActive(true); // Hide the regular score text
        }
    }
}