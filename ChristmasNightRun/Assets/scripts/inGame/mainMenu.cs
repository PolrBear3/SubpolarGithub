using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenu : MonoBehaviour
{
    public Animator transition;

    public void playGame()
    {
        // reloads current score back to 0 when exiting game
        scoreManager.score = 0.0f;

        transition.SetTrigger("Start");
        LoadnextLevel();

        ////takes away current life
        lifeForAds.currentLives -= 1;
        PlayerPrefs.SetInt("Lives", lifeForAds.currentLives);
        FindObjectOfType<audioManager>().Play("CurtainSound");
    }

    public void LoadnextLevel()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        yield return new WaitForSeconds(1.3f);
        SceneManager.LoadScene(levelIndex);
    }

    public void inventory()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }
    
    public void back()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 2);
    }
    
    public void toBag2()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void toBag3()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }

    public void toBag4()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 3);
    }

    public void toBag5()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 4);
    }

    public void shop()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 7);
    }

    public void shopBack()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 7);
        coinText.coinSave();
    }

    public void credit()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 8);
    }

    public void creditBack()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 8);
    }
}
