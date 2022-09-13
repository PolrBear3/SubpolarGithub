using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnPlanted_Menu : MonoBehaviour
{
    public MainGame_Controller controller;
    RectTransform rectTransform;
    public LeanTweenType tweenType;
    public Button[] allAvailableButtons;
    public GameObject[] buttonPages;

    public GameObject[] toolTipPanels;
    public Image[] toolTipSprites;
    public Text[] seedToolTipTexts;
    public Text[] buffToolTipTexts;

    public Image currentCropImage, currentBuffImage;
    public Sprite nullCropSprite, nullBuffSprite;
    public Seed_ScrObj currentSeedInfo;
    public Buff_ScrObj currentBuffInfo;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    private void Start()
    {
        buttonPages[1].SetActive(false);
        buttonPages[3].SetActive(false);
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
        Goto_Seed_Selection();
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

    public void Goto_Seed_Selection()
    {
        buttonPages[0].SetActive(true);
        buttonPages[1].SetActive(false);

        buttonPages[2].SetActive(true);
        buttonPages[3].SetActive(false);
    }
    public void Goto_Buff_Selection()
    {
        buttonPages[0].SetActive(false);
        buttonPages[1].SetActive(true);

        buttonPages[2].SetActive(false);
        buttonPages[3].SetActive(true);
    }

    private void Reset_Selections()
    {
        currentSeedInfo = null;
        currentCropImage.sprite = nullCropSprite;
        currentBuffInfo = null;
        currentBuffImage.sprite = nullBuffSprite;
    }
    private void Check_Selections_Complete()
    {
        if (currentSeedInfo != null && currentBuffInfo != null)
        {
            allAvailableButtons[0].enabled = true;
        }
    }
    public void Select_Seed(Seed_ScrObj seedInfo)
    {
        currentSeedInfo = seedInfo;
        currentCropImage.sprite = seedInfo.sprites[3];
        Check_Selections_Complete();
    }
    public void Select_Buff(Buff_ScrObj buffInfo)
    {
        currentBuffInfo = buffInfo;
        currentBuffInfo.sprite = buffInfo.sprite;
        Check_Selections_Complete();
    }

    private void Seed_Price_Calculation()
    {
        for (int i = 0; i < controller.farmTiles.Length; i++)
        {
            if (controller.openedTileNum == controller.farmTiles[i].tileNum)
            {
                if (controller.money >= currentSeedInfo.seedBuyPrice)
                {
                    PlantSeed_Close();
                    controller.Subtract_Money(currentSeedInfo.seedBuyPrice);
                    controller.farmTiles[i].image.sprite = currentSeedInfo.sprites[0];
                    controller.farmTiles[i].seedPlanted = true;
                    controller.farmTiles[i].plantedSeed = currentSeedInfo;
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
    private void SeedBuff_Price_Calculation()
    {
        for (int i = 0; i < controller.farmTiles.Length; i++)
        {
            if (controller.openedTileNum == controller.farmTiles[i].tileNum)
            {
                if (controller.money >= currentSeedInfo.seedBuyPrice + currentBuffInfo.buffPrice)
                {
                    PlantSeed_Close();
                    controller.Subtract_Money(currentSeedInfo.seedBuyPrice);
                    controller.farmTiles[i].image.sprite = currentSeedInfo.sprites[0];
                    controller.farmTiles[i].seedPlanted = true;
                    controller.farmTiles[i].plantedSeed = currentSeedInfo;
                    controller.farmTiles[i].selectedBuff = currentBuffInfo;
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
            if (currentBuffInfo == null)
            {
                Seed_Price_Calculation();
            }
            else
            {
                SeedBuff_Price_Calculation();
            }

            controller.eventSystem.All_Events_Check();
        }
    }

    // tool tip information order > sprite, name, description, price, sellPrice, average grow day
    public void Show_Seed_ToolTip(Seed_ScrObj currentHoveringSeed)
    {
        toolTipSprites[0].sprite = currentHoveringSeed.sprites[3];
        seedToolTipTexts[0].text = currentHoveringSeed.seedName;
        seedToolTipTexts[1].text = currentHoveringSeed.seedDescription;
        seedToolTipTexts[2].text = "seed price: $ " + currentHoveringSeed.seedBuyPrice.ToString();
        seedToolTipTexts[3].text = "sell price: $ " + currentHoveringSeed.harvestSellPrice.ToString();
        seedToolTipTexts[4].text = "harvest: " + 
        currentHoveringSeed.minFinishDays + "~" + currentHoveringSeed.maxFinishDays + " days".ToString();

        toolTipPanels[0].SetActive(true);
    }
    public void hide_Seed_ToolTip()
    {
        toolTipPanels[0].SetActive(false);
    }

    public void Show_Buff_ToolTip(Buff_ScrObj currenthoveringBuff)
    {
        toolTipSprites[1].sprite = currenthoveringBuff.sprite;
        buffToolTipTexts[0].text = currenthoveringBuff.buffName;
        buffToolTipTexts[1].text = currenthoveringBuff.description;
        buffToolTipTexts[2].text = "buff price: $ " + currenthoveringBuff.buffPrice.ToString();

        toolTipPanels[1].SetActive(true);
    }
    public void Hide_Buff_ToolTip()
    {
        toolTipPanels[1].SetActive(false);
    }
}
