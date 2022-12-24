using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileBuy_Button : MonoBehaviour
{
    public MainGame_Controller controller;

    public Text tilePrice;

    public void Open()
    {
        gameObject.SetActive(true);
        tilePrice.text = "$ " + controller.farmTiles[controller.openedTileNum].data.tilePrice.ToString();
    }
    public void Close()
    {
        gameObject.SetActive(false);
        controller.Reset_All_Tile_Highlights();
    }

    public void Buy_Tile()
    {
        var selectedTile = controller.farmTiles[controller.openedTileNum];

        if (controller.money >= selectedTile.data.tilePrice)
        {
            controller.Subtract_Money(selectedTile.data.tilePrice);
            selectedTile.Unlock_Tile();
            Close();
        }
        else
        {
            controller.defaultMenu.NotEnoughMoney_Blink_Tween();
        }

        controller.eventSystem.Activate_All_Events();
    }
}
