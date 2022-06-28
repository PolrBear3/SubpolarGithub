using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FarmTile : MonoBehaviour
{
    MainGame_Controller controller;

    Button button;
    Image image;

    public int tileNum;
    public bool seedPlanted = false;

    public Seed_ScrObj plantSeed = null;

    public void Awake()
    {
        controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<MainGame_Controller>();
        button = GetComponent<Button>();
        image = GetComponent<Image>();
    }

    public void Lock_Tile()
    {
        button.enabled = false;
        // locked tile image
        image.enabled = false;
    }
    public void Unlock_Tile()
    {
        button.enabled = true;
        // normal tile image
        image.enabled = true;
    }

    public void Open_Menu(FarmTile farmTile)
    {
        // detecting which tile was pressed
        controller.Set_OpenTileNum(farmTile);

        // reset menu
        Close_Menu();

        if (!seedPlanted)
        {
            controller.unPlantMenu.Open();
        }
        else if (seedPlanted)
        {
            Debug.Log("planted menu opened!");
        }
    }
    public void Close_Menu()
    {
        controller.unPlantMenu.Close();
        Debug.Log("planted menu closed!");
    }
}
