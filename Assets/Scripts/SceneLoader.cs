using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static int difficulty = 0; 

    public void OnEasyButton()
    {
        difficulty = 1;
        SceneManager.LoadScene("GameScene");
    }

    public void OnMediumButton()
    {
        difficulty = 2;
        SceneManager.LoadScene("GameScene");
    }

    public void OnHardButton()
    {
        difficulty = 3;
        SceneManager.LoadScene("GameScene");
    }

    public void OnBackToMenuButton()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void OnExitButton()
    {
        Application.Quit();
    }

}