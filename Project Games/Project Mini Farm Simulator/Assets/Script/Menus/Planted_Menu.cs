using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Planted_Menu_Data
{
    public bool menuOn = false;
}

[System.Serializable]
public class Planted_Menu_UI
{
    public Image plantedSeedImage;
    public Text plantedDayPassed,
                currentSeedHealth,
                currentWaterHealth;
    public GameObject[] harvestButtons;
    public GameObject[] BuffButtons;
}

[System.Serializable]
public class Current_Buffs_Panel_UI
{
    [HideInInspector]
    public bool cbON = false;
    [HideInInspector]
    public int openedActiveFrameNum;
    public RectTransform cbRT;
    public GameObject cbButton, cbPanel;
    public Current_Buff_Icon_UI[] icons;
}

[System.Serializable]
public class Current_Status_Panel_UI
{
    [HideInInspector]
    public bool panelOn = false;
    public GameObject panel;
    public GameObject button;
    public Status_Icon_Indicator statusIconController;
}

public class Planted_Menu : MonoBehaviour
{
    public MainGame_Controller controller;
    RectTransform rectTransform;
    public LeanTweenType tweenType;
    public Button[] allAvailableButtons;

    public Planted_Menu_Data data;
    public Planted_Menu_UI ui;
    public Current_Buffs_Panel_UI currentBuffsPanel;
    [SerializeField] private Current_Status_Panel_UI currentStatusPanel;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    private void Start()
    {
        // set to position at the center
        rectTransform.anchoredPosition = new Vector2(0f, -125f);

        // active buff panel frame number set to null
        Buff_ToolTip_Reset_FrameNum();
    }

    public void Button_Shield(bool activate)
    {
        for (int i = 0; i < allAvailableButtons.Length; i++)
        {
            if (activate) { allAvailableButtons[i].enabled = false; }
            else if (!activate) { allAvailableButtons[i].enabled = true; }
        }
    }

    public void Seed_Information_Update()
    {
        CurrentBuffs_Button_Check();
        StatusButton_Available_Check();

        var currentFarmTile = controller.farmTiles[controller.openedTileNum];
        if (currentFarmTile.data.seedPlanted)
        {
            ui.plantedDayPassed.text = "day " + currentFarmTile.tileSeedStatus.dayPassed.ToString();
            ui.currentSeedHealth.text = currentFarmTile.tileSeedStatus.health.ToString();
            ui.plantedSeedImage.sprite = currentFarmTile.data.plantedSeed.sprites[3];
        }

        var waterHealthCalculation = 
            currentFarmTile.data.plantedSeed.waterHealth - currentFarmTile.tileSeedStatus.daysWithoutWater;
        ui.currentWaterHealth.text = waterHealthCalculation.ToString();

        if (currentFarmTile.tileSeedStatus.dayPassed >= currentFarmTile.tileSeedStatus.fullGrownDay)
        {
            HarvestButton_Available();
        }
        else
        {
            HarvestButton_UnAvailable();
        }
    }
    
    public void Open()
    {
        data.menuOn = true;
        Button_Shield(false);
        Seed_Information_Update();
        LeanTween.move(rectTransform, new Vector2(0f, 104.85f), 0.75f).setEase(tweenType);
    }
    public void Close()
    {
        data.menuOn = false;
        controller.unPlantedMenu.Hide_Seed_ToolTip();
        Button_Shield(true);
        controller.Reset_All_Tile_Highlights();
        LeanTween.move(rectTransform, new Vector2(0f, -125f), 0.75f).setEase(tweenType);
        Close_CurrentBuffs_Panel();
        Close_CurrentStatus_Panel();
    }
    public void Open_BuffMenu()
    {
        Button_Shield(true);
        controller.buffMenu.Open();
    }

    public void Remove_Seed()
    {
        var currentFarmTile = controller.farmTiles[controller.openedTileNum];
        controller.Add_Money(currentFarmTile.data.plantedSeed.seedBuyPrice);
        controller.timeSystem.Subtract_MyTime(currentFarmTile.data.plantedSeed.waitTime);
        currentFarmTile.Reset_Tile();
        controller.eventSystem.Activate_All_Events();
        controller.plantedMenu.Close();
    }

    public void Water_Seed()
    {
        var currentFarmTile = controller.farmTiles[controller.openedTileNum];
        if (!currentFarmTile.tileSeedStatus.currentDayWatered)
        {
            currentFarmTile.tileSeedStatus.currentDayWatered = true;
            currentFarmTile.tileSeedStatus.watered += 1;
            currentFarmTile.tileSeedStatus.daysWithoutWater = 0;
            currentFarmTile.Add_Status(0);

            // if the seed is fully grown to crop, watering doesn't count
            if (currentFarmTile.tileSeedStatus.dayPassed >= currentFarmTile.tileSeedStatus.fullGrownDay)
            {
                currentFarmTile.tileSeedStatus.watered -= 1;
            }
        }

        // water health text update
        var waterHealthCalculation =
            currentFarmTile.data.plantedSeed.waterHealth - currentFarmTile.tileSeedStatus.daysWithoutWater;
        ui.currentWaterHealth.text = waterHealthCalculation.ToString();

        StatusButton_Available_Check();
    }

    private void HarvestButton_Available()
    {
        ui.harvestButtons[1].SetActive(true);
        ui.harvestButtons[0].SetActive(false);
    }
    private void HarvestButton_UnAvailable()
    {
        ui.harvestButtons[1].SetActive(false);
        ui.harvestButtons[0].SetActive(true);
    }
    
    // all the individual bonus infos
    private int Watering_Bonus()
    {
        var currentFarmTile = controller.farmTiles[controller.openedTileNum].tileSeedStatus;

        // tier 1
        if (currentFarmTile.watered >= currentFarmTile.fullGrownDay)
        {
            return 3;
        }
        // tier 2
        else if (currentFarmTile.watered >= currentFarmTile.fullGrownDay * 0.75f)
        {
            return 2;
        }
        // tier 3
        else if (currentFarmTile.watered >= currentFarmTile.fullGrownDay * 0.5f)
        {
            return 1;
        }
        // tier 4
        else return 0;
    }
    private int Tile_Point_Bonus()
    {
        var currentFarmTile = controller.farmTiles[controller.openedTileNum];
        return currentFarmTile.tileSeedStatus.bonusPoints;
    }
    
    // all bonus infos add to one sum
    private int All_Bonus_Sum()
    {
        return Watering_Bonus() + Tile_Point_Bonus();
    }
    
    // calculate bonus points, give player money, reset the tile
    public void Harvest_Sell()
    {
        var currentFarmTile = controller.farmTiles[controller.openedTileNum];

        // calculate total
        if (All_Bonus_Sum() > 0)
        {
            controller.Add_Money_withBonus(currentFarmTile.data.plantedSeed.harvestSellPrice, All_Bonus_Sum());
        }
        else if (All_Bonus_Sum() == 0)
        {
            controller.Add_Money(currentFarmTile.data.plantedSeed.harvestSellPrice);
        }
        currentFarmTile.Reset_Tile();
        controller.eventSystem.Activate_All_Events();
        currentFarmTile.harvestCoinsAnim.SetBool("harvest", true);

        Close();
    }

    // current buffs for planted seed panel functions
    public void CurrentBuffs_Button_Check()
    {
        var currentFarmTile = controller.farmTiles[controller.openedTileNum];

        if (currentFarmTile.currentBuffs.Count > 0)
        {
            currentBuffsPanel.cbButton.SetActive(true);
        }
        else
        {
            currentBuffsPanel.cbButton.SetActive(false);
        }
    }
    
    private void CheckSpace_Assign(Buff_ScrObj buff)
    {
        var x = currentBuffsPanel.icons;
        for (int i = 0; i < x.Length; i++)
        {
            if (!x[i].hasBuff)
            {
                x[i].Assign_Icon(buff);
                break;
            }
        }
    }
    private void CurrentBuffs_Assign_Icon()
    {
        // reset all icons for re order 
        for (int i = 0; i < currentBuffsPanel.icons.Length; i++)
        {
            currentBuffsPanel.icons[i].Empty_Icon();
        }
        
        var currentFarmTile = controller.farmTiles[controller.openedTileNum];
        var buffAmount = currentFarmTile.currentBuffs.Count;

        // assign icon indications in farmtile buff list if it has at least 1 buff
        if (buffAmount > 0)
        {
            for (int i = 0; i < currentFarmTile.currentBuffs.Count; i++)
            {
                CheckSpace_Assign(currentFarmTile.currentBuffs[i]);
            }
        }
    }
    
    public void Open_Close_CurrentBuffs_Panel()
    {
        if (!currentBuffsPanel.cbON)
        {
            currentBuffsPanel.cbON = true;
            CurrentBuffs_Assign_Icon();
            currentBuffsPanel.cbPanel.SetActive(true);

            // close current status panel if it is on
            if (!currentStatusPanel.panelOn) return;
            Close_CurrentStatus_Panel();
        }
        else
        {
            Close_CurrentBuffs_Panel();
        }
    }
    public void Close_CurrentBuffs_Panel()
    {
        currentBuffsPanel.cbON = false;
        currentBuffsPanel.cbPanel.SetActive(false);
        Buff_ToolTip_Reset_FrameNum();
        controller.buffMenu.Hide_Buff_ToolTip();
    }

    public void Buff_ToolTip_Reset_FrameNum()
    {
        currentBuffsPanel.openedActiveFrameNum = -1;
    }
    public void Show_Hide_Buff_ToolTip(int frameNum)
    {
        var currentFarmTile = controller.farmTiles[controller.openedTileNum];
        var x = controller.buffMenu;

        // if active buff is not empty
        if (currentFarmTile.currentBuffs[frameNum] == null) return;

        // if currently opened buff is pressed again, close the tooltip
        if (frameNum == currentBuffsPanel.openedActiveFrameNum)
        {
            x.Hide_Buff_ToolTip();
            Buff_ToolTip_Reset_FrameNum();
            return;
        }

        // open active buff tooltip
        x.Show_ActiveBuff_ToolTip(currentFarmTile.currentBuffs[frameNum]);
        currentBuffsPanel.openedActiveFrameNum = frameNum;
    }

    // current seed status panel functions
    public void StatusButton_Available_Check()
    {
        var openedTile = controller.farmTiles[controller.openedTileNum];

        // if the farmTile has at least 1 or more status
        if (openedTile.currentStatuses.Count > 0)
        {
            currentStatusPanel.button.SetActive(true);
        }
        else
        {
            currentStatusPanel.button.SetActive(false);
        }
    }

    public void Open_Close_CurrentStatus_Panel()
    {
        // on
        if (!currentStatusPanel.panelOn)
        {
            currentStatusPanel.panelOn = true;
            currentStatusPanel.panel.SetActive(true);
            currentStatusPanel.statusIconController.Reset_Status_Icons();
            currentStatusPanel.statusIconController.Update_CurrentFarmTile_Status();

            // close current buffs panel if it is on
            if (!currentBuffsPanel.cbON) return;
            Close_CurrentBuffs_Panel();
        }
        // off
        else
        {
            currentStatusPanel.panelOn = false;
            currentStatusPanel.panel.SetActive(false);
        }
    }
    public void Close_CurrentStatus_Panel()
    {
        currentStatusPanel.panelOn = false;
        currentStatusPanel.panel.SetActive(false);
    }

    // planted seed tooltip function
    public void Show_Hide_Seed_ToolTip()
    {
        var x = controller.unPlantedMenu.seedToolTipUIconnection;

        if (!x.toolTipOn)
        {
            controller.unPlantedMenu.Show_PlanedSeed_ToolTip(controller.farmTiles[controller.openedTileNum].data.plantedSeed);
        }
        else
        {
            controller.unPlantedMenu.Hide_Seed_ToolTip();
        }
    }
}
