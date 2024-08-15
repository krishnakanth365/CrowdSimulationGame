using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject gameLostUI;
    public GameObject gameWonUI;

    private void Awake()
    {
        instance = this;
    }

    public void GameWonScreen(int Strikes)
    {
        Cursor.lockState = CursorLockMode.None;
        gameWonUI.SetActive(true);
        Time.timeScale = 0;
    }

    public void GameLostScreen()
    {
        Cursor.lockState = CursorLockMode.None;
        gameLostUI.SetActive(true);
        Time.timeScale = 0;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
