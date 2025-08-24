using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Holds all the functions for the main menu
/// </summary>

public class MainMenuScript : MonoBehaviour
{

    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(1);
    }
    public void QuitGame()
    {
#if UNITY_STANDALONE
            Application.Quit();
#endif
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

}
