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
    [SerializeField] private MainGame_Controller controller;
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
        controller.Reset_All_Tile_Highlights();
        Button_Shield(true);
        LeanTween.move(ui.rectTransform, new Vector2(0f, -125f), 0.75f).setEase(data.tweenType);
    }

    public void Open_Buff_History_Panel()
    {
        data.buffPanelOn = true;
        buffPanel.gameObject.SetActive(true);
    }
    private void Close_Buff_History_Panel()
    {
        data.buffPanelOn = false;
        buffPanel.gameObject.SetActive(false);
    }

    private int Damage_Amount(FarmTile farmTile)
    {
        var statuses = farmTile.currentStatuses;
        int damageAmount = 0;

        for (int i = 0; i < statuses.Count; i++)
        {
            if (statuses[i] == null) break;

            // if it died from drying out 
            if (statuses[i].statusID == 10)
            {
                damageAmount += farmTile.deathData.previousHealth;
            }
            else if (statuses[i].healthValue > 0)
            {
                damageAmount += statuses[i].healthValue;
            }
            else if (statuses[i].healthValue < 0)
            {
                damageAmount -= statuses[i].healthValue;
            }
        }

        return damageAmount;
    }
    private void Update_Death_Info()
    {
        var currentFarmTile = controller.farmTiles[controller.openedTileNum];

        // previous planted seed image
        ui.previousSeedImage.sprite = currentFarmTile.deathData.previousSeed.sprites[3];

        // health
        ui.previouHealthText.text = currentFarmTile.deathData.previousHealth.ToString();
        int deathHealthAmount = currentFarmTile.deathData.previousHealth - Damage_Amount(currentFarmTile);
        ui.deathHealthText.text = deathHealthAmount.ToString();

        // current statuses
        statusPanel.Assign_All(currentFarmTile);
    }
}
