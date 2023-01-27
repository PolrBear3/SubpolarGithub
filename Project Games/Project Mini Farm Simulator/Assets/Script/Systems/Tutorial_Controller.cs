using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_Controller : MonoBehaviour
{
    [SerializeField] private MainGame_Controller controller;

    [SerializeField] private GameObject optionsMenuTutorialScreen;
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

        // last guide screen check
        if (tutorialScreens.Length - currentScreenNum == 1)
        {
            controller.Reset_All_Menu();
        }

        // tutorial end check
        if (currentScreenNum >= tutorialScreens.Length)
        {
            Destroy(gameObject);
            return;
        }
        
        // go to next guide screen
        tutorialScreens[currentScreenNum].SetActive(true);
    }

    public void Press_OpitonsMenu_DuringTutorial()
    {
        if (controller.optionsMenu.data.menuOn)
        {
            // turn off current guide screen 
            tutorialScreens[currentScreenNum].SetActive(false);

            // turn on options menu tutorial screen
            optionsMenuTutorialScreen.SetActive(true);
        }
        else
        {
            // turn off options menu tutorial screen
            optionsMenuTutorialScreen.SetActive(false);

            // turn on current guide screen 
            tutorialScreens[currentScreenNum].SetActive(true);
        }
    }
}
