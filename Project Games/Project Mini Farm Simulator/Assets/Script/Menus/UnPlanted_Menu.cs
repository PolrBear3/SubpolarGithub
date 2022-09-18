using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Unplanted_Menu_Seed_ToolTip
{
    public GameObject toolTipPanel;
    public Image previewSeedSprite;
    public Text seedName, seedDescription, seedPrice, sellPrice, harvestLength;
}

public class UnPlanted_Menu : MonoBehaviour
{
    public MainGame_Controller controller;
    RectTransform rectTransform;
    public LeanTweenType tweenType;
    public Button[] allAvailableButtons;

    public Unplanted_Menu_Seed_ToolTip seedToolTipUIconnection;
    public Image currentCropImage;
    public Seed_ScrObj currentSeedInfo;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Button_Shield(bool activate)
    {
        for (int i = 0; i < allAvailableButtons.Length; i++)
        {
            if (activate) { allAvailableButtons[i].enabled = false; }
            else if (!activate) { allAvailableButtons[i].enabled = true; }
        }
    }

    public void Open()
    {
        Button_Shield(false);
        LeanTween.move(rectTransform, new Vector2(0f, 104.85f), 0.75f).setEase(tweenType);
    }
    public void Close()
    {
        Reset_Selections();
        Button_Shield(true);
        controller.Reset_All_Tile_Highlights();
        LeanTween.move(rectTransform, new Vector2(0f, -125f), 0.75f).setEase(tweenType);
    }
    public void PlantSeed_Close()
    {
        Button_Shield(true);
        LeanTween.move(rectTransform, new Vector2(0f, -125f), 0.75f).setEase(tweenType);
    }

    private void Reset_Selections()
    {
        currentSeedInfo = null;
        currentCropImage.color = Color.clear;
    }
    private void Check_Selections_Complete()
    {
        if (currentSeedInfo != null)
        {
            allAvailableButtons[0].enabled = true;
        }
    }
    public void Select_Seed(Seed_ScrObj seedInfo)
    {
        currentSeedInfo = seedInfo;
        currentCropImage.sprite = seedInfo.sprites[3];
        currentCropImage.color = Color.white;
        Check_Selections_Complete();
    }

    private void Seed_Price_Calculation()
    {
        for (int i = 0; i < controller.farmTiles.Length; i++)
        {
            if (controller.openedTileNum == controller.farmTiles[i].data.tileNum)
            {
                if (controller.money >= currentSeedInfo.seedBuyPrice)
                {
                    PlantSeed_Close();
                    controller.Subtract_Money(currentSeedInfo.seedBuyPrice);
                    controller.farmTiles[i].image.sprite = currentSeedInfo.sprites[0];
                    controller.farmTiles[i].data.seedPlanted = true;
                    controller.farmTiles[i].data.plantedSeed = currentSeedInfo;
                    controller.farmTiles[i].Seed_Planted_Start_Set();
                    controller.plantedMenu.Open();
                    Reset_Selections();
                    break;
                }
                else
                {
                    controller.defaultMenu.NotEnoughMoney_Blink_Tween();
                }
            }
        }
    }
    public void Plant_Seed()
    {
        if (currentSeedInfo != null)
        {
            Seed_Price_Calculation();
        }
    }

    public void Show_Seed_ToolTip(Seed_ScrObj currentHoveringSeed)
    {
        var x = seedToolTipUIconnection;
        x.previewSeedSprite.sprite = currentHoveringSeed.sprites[3];
        x.seedName.text = currentHoveringSeed.seedName;
        x.seedDescription.text = currentHoveringSeed.seedDescription;
        x.seedPrice.text = "seed price: $ " + currentHoveringSeed.seedBuyPrice.ToString();
        x.sellPrice.text = "sell price: $ " + currentHoveringSeed.harvestSellPrice.ToString();
        x.harvestLength.text = "harvest: " +
        currentHoveringSeed.minFinishDays + "~" + currentHoveringSeed.maxFinishDays + " days".ToString();

        x.toolTipPanel.SetActive(true);
    }
    public void hide_Seed_ToolTip()
    {
        seedToolTipUIconnection.toolTipPanel.SetActive(false);
    }
}
