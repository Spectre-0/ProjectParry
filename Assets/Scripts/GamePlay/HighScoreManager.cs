using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HighScoreManager
{
    public static int GetHighScoreForLevel(int levelIndex)
    {
        return PlayerPrefs.GetInt($"Level{levelIndex}HighScore", 0);
    }

    public static void SetHighScoreForLevel(int levelIndex, int value)
    {
        PlayerPrefs.SetInt($"Level{levelIndex}HighScore", value);
        PlayerPrefs.Save(); // Save the change immediately
    }
}
