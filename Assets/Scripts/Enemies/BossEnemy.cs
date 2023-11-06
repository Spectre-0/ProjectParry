using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class BossEnemy : MonoBehaviour
{
    public GameObject youWinText; // Assign your 'You Win' UI Text element in the inspector

    private void OnDestroy()
    {
        // Activate the 'You Win' text
        youWinText.SetActive(true);

        Cursor.lockState = CursorLockMode.None;

        SceneManager.LoadScene(0);

        // Start coroutine to load the scene after a delay
        StartCoroutine(LoadSceneAfterDelay());
    }

    private IEnumerator LoadSceneAfterDelay()
    {
        // Wait for 3 seconds
        yield return new WaitForSeconds(3);

        // Load scene at index 0
        SceneManager.LoadScene(0);
    }
}