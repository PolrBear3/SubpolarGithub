using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class pauseController : MonoBehaviour
{
    public static bool gameIsPaused = false;
    public GameObject pauseMenu;
    public GameObject pauseButton;

    public void gamePause()
    {
        pauseTrigger();

        //pause for rest of things
        pauseMenu.SetActive(true);
        pauseButton.SetActive(false);
        Time.timeScale = 0f;
        gameIsPaused = true;

        playerForMobile.audioSRC.Stop();
    }

    // pause/play for player and camera and snow effect box
    public void pauseTrigger()
    {
        GameState currentGameState = GameStateManager.Instance.CurrentGameState;
        GameState newGameState = currentGameState == GameState.GamePlay
            ? GameState.Paused
            : GameState.GamePlay;
        GameStateManager.Instance.SetState(newGameState);
    }
    
    public void Resume()
    {
        pauseTrigger();

        pauseMenu.SetActive(false);
        pauseButton.SetActive(true);
        Time.timeScale = 1f;

        playerForMobile.audioSRC.Play();
    }

    public void Restart()
    {
        //takes away current life
        lifeForAds.currentLives -= 1;
        PlayerPrefs.SetInt("Lives", lifeForAds.currentLives);
        
        //turns on pause menu
        pauseMenu.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 0);
        scoreManager.score = 0.0f;
        pauseTrigger();
        Time.timeScale = 1f;
    }

    public void MainMenu()
    {
        PlayerPrefs.SetInt("Lives", lifeForAds.currentLives);
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        pauseTrigger();
    }
}
