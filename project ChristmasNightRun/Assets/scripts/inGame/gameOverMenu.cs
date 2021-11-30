using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gameOverMenu : MonoBehaviour
{
    public static bool gameOver;
    public GameObject gameOverPenal;

    void Start()
    {
        gameOver = false;
    }

    void Update()
    {
        if (gameOver)
        {
            gameOverPenal.SetActive(true);
        }
    }
}
