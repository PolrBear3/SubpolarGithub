using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class scoreManager : MonoBehaviour
{
    public static float score = 0.0f;
    public Text scoreText;
    public Text highScoreText;

    void Start()
    {
        highScoreText.text = "" + ((int)PlayerPrefs.GetFloat("Highscore")).ToString();
    }

    void Update()
    {
        score += Time.deltaTime;
        scoreText.text = ((int)score).ToString();
    }

    public static void highscoreSave()
    {
        if(PlayerPrefs.GetFloat("Highscore") < score)
        {
            PlayerPrefs.SetFloat("Highscore", score);
        }
    }

    public static void GameOver()
    {
        if (gameOverMenu.gameOver)
        {
            Time.timeScale = 0;
        }
    }

    public void backToMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void playAgain()
    {
        scoreManager.score = 0.0f;
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 0);
    }
}
