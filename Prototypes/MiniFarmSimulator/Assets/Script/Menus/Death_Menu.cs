using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct Death_Menu_Data
{
    public LeanTweenType tweenType;
    public bool buffPanelOn;
}

[System.Serializable]
public class Death_Menu_UI
{
    public RectTransform rectTransform;
    public Button[] allAvailableButtons;

    public Image previousSeedImage;
    public Text previouHealthText;
    public Text deathHealthText;
}

public class Death_Menu : MonoBehaviour
{
    public MainGame_Controller controller;
    [SerializeField] private History_Status_Panel statusPanel;
    [SerializeField] private History_Buff_Panel buffPanel;

    [SerializeField] private Death_Menu_Data data;
    [SerializeField] private Death_Menu_UI ui;

    private void Start()
    {
        // set to position at the center
        ui.rectTransform.anchoredPosition = new Vector2(0f, -125f);
    }

    // button shields
    public void Button_Shield(bool activate)
    {
        for (int i = 0; i < ui.allAvailableButtons.Length; i++)
        {
            if (activate) { ui.allAvailableButtons[i].enabled = false; }
            else if (!activate) { ui.allAvailableButtons[i].enabled = true; }
        }
    }

    public void Open()
    {
        Button_Shield(false);
        LeanTween.move(ui.rectTransform, new Vector2(0f, 104.85f), 0.75f).setEase(data.tweenType);

        var currentFarmTile = controller.farmTiles[controller.openedTileNum];
        currentFarmTile.deathData.died = false;
        currentFarmTile.deathIcon.SetActive(false);

        Update_Death_Info();
    }
    public void Close()
    {
        // close seed tooltip
        controller.unPlantedMenu.Hide_Seed_ToolTip();

        Close_Buff_History_Panel();
        statusPanel.Hide_Status_ToolTip();

        controller.Reset_All_Tile_Highlights();
        Button_Shield(true);
        LeanTween.move(ui.rectTransform, new Vector2(0f, -125f), 0.75f).setEase(data.tweenType);
    }
    // close for pressing close menu button in death menu
    public void InMenu_Close()
    {
        // close seed tooltip
        controller.unPlantedMenu.Hide_Seed_ToolTip();

        if (data.buffPanelOn)
        {
            Close_Buff_History_Panel();
            return;
        }
        statusPanel.Hide_Status_ToolTip();

        controller.Reset_All_Tile_Highlights();
        Button_Shield(true);
        LeanTween.move(ui.rectTransform, new Vector2(0f, -125f), 0.75f).setEase(data.tweenType);
    }

    private void Open_Buff_History_Panel()
    {
        if (data.buffPanelOn) return;

        data.buffPanelOn = true;
        buffPanel.gameObject.SetActive(true);

        var currentFarmTile = controller.farmTiles[controller.openedTileNum];
        buffPanel.Assign_All(currentFarmTile);

        statusPanel.Hide_Status_ToolTip();
    }
    private void Close_Buff_History_Panel()
    {
        if (!data.buffPanelOn) return;

        data.buffPanelOn = false;
        buffPanel.gameObject.SetActive(false);
        controller.buffMenu.Hide_Buff_ToolTip();
        controller.plantedMenu.Hide_Buff_ToolTip();
    }
    public void OpenClose_Buff_History_Panel()
    {
        // close seed tooltip
        controller.unPlantedMenu.Hide_Seed_ToolTip();

        if (!data.buffPanelOn)
        {
            Open_Buff_History_Panel();
        }
        else
        {
            Close_Buff_History_Panel();
        }
    }

    private void Update_Death_Info()
    {
        var currentFarmTile = controller.farmTiles[controller.openedTileNum];

        // previous planted seed image
        ui.previousSeedImage.sprite = currentFarmTile.deathData.previousSeed.sprites[3];

        // health
        ui.previouHealthText.text = currentFarmTile.deathData.previousHealth.ToString();
        int deathHealthAmount = currentFarmTile.deathData.previousHealth + currentFarmTile.deathData.damageCount;
        ui.deathHealthText.text = deathHealthAmount.ToString();

        // current statuses
        statusPanel.Assign_All(currentFarmTile);
    }

    // seed tooltip
    public void Death_Seed_ToolTip()
    {
        if (!controller.unPlantedMenu.seedToolTipUIconnection.toolTipOn)
        {
            statusPanel.Hide_Status_ToolTip();
            controller.unPlantedMenu.Show_PlanedSeed_ToolTip(controller.farmTiles[controller.openedTileNum].deathData.previousSeed);
        }
        else
        {
            controller.unPlantedMenu.Hide_Seed_ToolTip();
        }
    }
}
