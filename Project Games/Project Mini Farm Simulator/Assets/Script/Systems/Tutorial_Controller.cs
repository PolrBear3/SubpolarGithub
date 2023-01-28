using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_Controller : MonoBehaviour
{
    [SerializeField] private MainGame_Controller controller;

    [SerializeField] private GameObject firstGameScreen;
    [SerializeField] private GameObject optionsMenuTutorialScreen;
    [SerializeField] private GameObject[] tutorialScreens;
    private int currentScreenNum = 0;

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

    public void Skip_TutorialGuide()
    {
        controller.saveSystem.data.tutorialComplete = true;
        Destroy(gameObject);
    }

    public void Start_Guide_Screen()
    {
        // close first game screen
        firstGameScreen.SetActive(false);

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
            controller.saveSystem.data.tutorialComplete = true;
            Destroy(gameObject);
            return;
        }
        
        // go to next guide screen
        tutorialScreens[currentScreenNum].SetActive(true);
    }

    public void Replay_TutorialGuide()
    {
        // turn off current las screen
        tutorialScreens[currentScreenNum].SetActive(false);

        // go back to first guide screen
        currentScreenNum = 0;
        Next_Guide_Screen();

        // reset to day 1
        controller.timeSystem.currentInGameDay = 1;
        controller.defaultMenu.Update_UI();

        // reset the collectable that was bought at tutorial
        controller.collectableRoomMenu.Reset_AllCollectables_Data();

        // lock guide farmtile
        var guideFarmTile = controller.farmTiles[0];
        guideFarmTile.Lock_Tile();

        // reset buff menu page to 1
        controller.buffMenu.pageController.FisrtPage();
    }

    public void Tutorial_FarmTile_Update()
    {
        var guideFarmTile = controller.farmTiles[0];

        guideFarmTile.tileSeedStatus.dayPassed = guideFarmTile.tileSeedStatus.fullGrownDay;
        guideFarmTile.tileSeedStatus.harvestReady = true;
        guideFarmTile.tileSeedStatus.health = 3;
        guideFarmTile.tileSeedStatus.bonusPoints = 4;
    }
}
