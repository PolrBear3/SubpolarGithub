using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Unplanted_Menu_Seed_ToolTip
{
    public bool toolTipOn = false;

    public GameObject toolTipPanel;
    public Animator toolTipAnimator;
    public Image previewSeedSprite;
    public Text seedName, seedDescription, seedPrice, sellPrice, harvestLength, seedMaxHealth, seedMaxWaterHealth;
}

[System.Serializable]
public class Unplanted_Menu_ButtonPage
{
    public int pageNum;
    public List<Seed_Button> buttons;
}

public class UnPlanted_Menu : MonoBehaviour
{
    public MainGame_Controller controller;
    private RectTransform rectTransform;
    public LeanTweenType tweenType;
    public Page_Controller pageController;

    public GameObject[] confirmButtons;

    public Button[] allAvailableButtons;
    public Unplanted_Menu_ButtonPage[] allButtonPages;

    private List<Seed_Button> currentButtons;

    public Unplanted_Menu_Seed_ToolTip seedToolTipUIconnection;
    public Image currentCropImage;
    public Seed_ScrObj currentSeedInfo;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    private void Start()
    {
        // set to position at the center
        rectTransform.anchoredPosition = new Vector2(0f, -125f);
        // seed button page set
        Set_Start_CurrentButtonPage();
    }

    private void Button_Shield(bool activate)
    {
        for (int i = 0; i < allAvailableButtons.Length; i++)
        {
            if (activate) { allAvailableButtons[i].enabled = false; }
            else if (!activate) { allAvailableButtons[i].enabled = true; }
        }
    }
    private void SeedButtons_Shield(bool activate)
    {
        for (int i = 0; i < currentButtons.Count; i++)
        {
            if (activate) { currentButtons[i].Button_Shield(true); }
            else if (!activate) { currentButtons[i].Button_Shield(false); }
        }
    }

    // basic functions
    public void Open()
    {
        Reset_Selections();
        Button_Shield(false);
        SeedButtons_Shield(false);
        LeanTween.move(rectTransform, new Vector2(0f, 104.85f), 0.75f).setEase(tweenType);
        Set_New_CurrentButtonPage();
    }
    public void Close()
    {
        hide_Seed_ToolTip();
        Reset_Selections();
        Button_Shield(true);
        SeedButtons_Shield(true);
        controller.Reset_All_Tile_Highlights();
        LeanTween.move(rectTransform, new Vector2(0f, -125f), 0.75f).setEase(tweenType);
    }
    public void PlantSeed_Close()
    {
        Button_Shield(true);
        SeedButtons_Shield(true);
        LeanTween.move(rectTransform, new Vector2(0f, -125f), 0.75f).setEase(tweenType);
    }

    // current button loop functions
    private void Set_Start_CurrentButtonPage()
    {
        currentButtons = allButtonPages[0].buttons;
    }
    public void Set_New_CurrentButtonPage()
    {
        // unselect all buttons for pressed sprite change
        Unpress_All_Buttons();

        // set new current button page
        currentButtons = allButtonPages[pageController.currentPageNum - 1].buttons;
        
        // update button ui information
        for (int i = 0; i < currentButtons.Count; i++)
        {
            currentButtons[i].Set_Seed_Info();
        }
    }

    // button pressed unpressed functions
    public void Unpress_All_Buttons()
    {
        for (int i = 0; i < currentButtons.Count; i++)
        {
            currentButtons[i].UnPress_This_Button();
        }
    }

    // distinctive functions
    public void Reset_Selections()
    {
        currentSeedInfo = null;
        currentCropImage.color = Color.clear;
        seedToolTipUIconnection.toolTipAnimator.SetBool("seedSelected", false);
        PlantSeed_Button_Availability();
    }
    
    private void PlantSeed_Button_Availability()
    {
        if (currentSeedInfo != null && controller.money >= currentSeedInfo.seedBuyPrice)
        {
            confirmButtons[0].SetActive(false);
            confirmButtons[1].SetActive(true);
        }
        else
        {
            confirmButtons[0].SetActive(true);
            confirmButtons[1].SetActive(false);
        }
    }
    public void Select_Seed(Seed_ScrObj seedInfo)
    {
        currentSeedInfo = seedInfo;
        currentCropImage.sprite = seedInfo.sprites[3];
        currentCropImage.color = Color.white;
        seedToolTipUIconnection.toolTipAnimator.SetBool("seedSelected", true);
        PlantSeed_Button_Availability();
    }
    public void Plant_Seed()
    {
        for (int i = 0; i < controller.farmTiles.Length; i++)
        {
            if (controller.openedTileNum == controller.farmTiles[i].data.tileNum)
            {
                PlantSeed_Close();
                controller.Subtract_Money(currentSeedInfo.seedBuyPrice);
                controller.farmTiles[i].image.sprite = currentSeedInfo.sprites[0];
                controller.farmTiles[i].data.seedPlanted = true;
                controller.farmTiles[i].data.plantedSeed = currentSeedInfo;
                controller.farmTiles[i].Seed_Planted_Start_Set();
                controller.plantedMenu.Open();
                controller.timeSystem.Add_MyTime(currentSeedInfo.waitTime);
                Reset_Selections();
                break;
            }
        }
    }

    // tool tips
    private void Show_Seed_ToolTip()
    {
        var x = seedToolTipUIconnection;

        if (currentSeedInfo != null)
        {
            x.toolTipOn = true;
            x.previewSeedSprite.sprite = currentSeedInfo.sprites[3];
            x.seedName.text = currentSeedInfo.seedName;
            x.seedDescription.text = currentSeedInfo.seedDescription;
            x.seedPrice.text = "seed price: $ " + currentSeedInfo.seedBuyPrice.ToString();
            x.sellPrice.text = "sell price: $ " + currentSeedInfo.harvestSellPrice.ToString();
            x.harvestLength.text = "harvest: " +
            currentSeedInfo.minFinishDays + "~" + currentSeedInfo.maxFinishDays + " days".ToString();
            x.seedMaxHealth.text = currentSeedInfo.seedHealth.ToString();
            x.seedMaxWaterHealth.text = currentSeedInfo.waterHealth.ToString();

            x.toolTipPanel.SetActive(true);
        }
    }
    public void Show_PlanedSeed_ToolTip(Seed_ScrObj planedSeed)
    {
        var x = seedToolTipUIconnection;

        x.toolTipOn = true;
        x.previewSeedSprite.sprite = planedSeed.sprites[3];
        x.seedName.text = planedSeed.seedName;
        x.seedDescription.text = planedSeed.seedDescription;
        x.seedPrice.text = "seed price: $ " + planedSeed.seedBuyPrice.ToString();
        x.sellPrice.text = "sell price: $ " + planedSeed.harvestSellPrice.ToString();
        x.harvestLength.text = "harvest: " +
        planedSeed.minFinishDays + "~" + planedSeed.maxFinishDays + " days".ToString();
        x.seedMaxHealth.text = planedSeed.seedHealth.ToString();
        x.seedMaxWaterHealth.text = planedSeed.waterHealth.ToString();

        x.toolTipPanel.SetActive(true);
    }
    public void hide_Seed_ToolTip()
    {
        seedToolTipUIconnection.toolTipOn = false;
        seedToolTipUIconnection.toolTipPanel.SetActive(false);
    }

    public void Show_Hide_ToolTip()
    {
        if (!seedToolTipUIconnection.toolTipOn)
        {
            Show_Seed_ToolTip();
        }
        else
        {
            hide_Seed_ToolTip();
        }
    }
}
