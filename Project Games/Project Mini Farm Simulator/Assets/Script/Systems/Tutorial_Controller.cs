using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_Controller : MonoBehaviour
{
    [SerializeField] private MainGame_Controller controller;

    [SerializeField] private GameObject[] tutorialScreens;
    private int currentScreenNum = 0;

    public void Start_Guide_Screen()
    {
        // set guide screen number to first page
        currentScreenNum = 0;
        
        // turn on current starting guide screen 
        tutorialScreens[currentScreenNum].SetActive(true);
    }

    public void Next_Guide_Screen()
    {
        // turn off current guide screen
        tutorialScreens[currentScreenNum].SetActive(false);
        
        // increase currentScreenNum
        currentScreenNum++;

        // tutorial end check
        if (currentScreenNum >= tutorialScreens.Length)
        {
            Destroy(gameObject);
            return;
        }
        
        // go to next guide screen
        tutorialScreens[currentScreenNum].SetActive(true);
    }
}
