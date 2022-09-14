using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Locked_Menu : MonoBehaviour
{
    public MainGame_Controller controller;

    RectTransform rectTransform;
    public LeanTweenType tweenType;
    public Button[] allAvailableButtons;

    public Text tilePrice;

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
        tilePrice.text = controller.farmTiles[controller.openedTileNum].tilePrice + " $".ToString();
        LeanTween.move(rectTransform, new Vector2(0f, 104.85f), 0.75f).setEase(tweenType);
    }
    public void Close()
    {
        Button_Shield(true);
        controller.Reset_All_Tile_Highlights();
        LeanTween.move(rectTransform, new Vector2(0f, -125f), 0.75f).setEase(tweenType);
    }

    public void Buy_Tile()
    {
        var selectedTile = controller.farmTiles[controller.openedTileNum];

        if (controller.money >= selectedTile.tilePrice)
        {
            controller.Subtract_Money(selectedTile.tilePrice);
            selectedTile.Unlock_Tile();
            controller.eventSystem.All_Events_Update_Check();
            Close();
        }
        else
        {
            controller.defaultMenu.NotEnoughMoney_Blink_Tween();
        }
    }
}
