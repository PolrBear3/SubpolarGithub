using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FarmTile : MonoBehaviour
{
    MainGame_Controller controller;

    [HideInInspector]
    public Image image;

    public int tileNum;
    public bool tileLocked = false, seedPlanted = false;
    public Sprite[] defaultTileSprites;

    public Seed_ScrObj plantSeed = null;

    public void Awake()
    {
        controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<MainGame_Controller>();
        image = GetComponent<Image>();
    }

    public void Lock_Tile()
    {
        tileLocked = true;

        // locked tile image
        image.sprite = defaultTileSprites[0];
    }
    public void Unlock_Tile()
    {
        tileLocked = false;

        image.sprite = defaultTileSprites[1];
    }

    public void Highlight_Tile()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
    }
    public void UnHighlight_Tile()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void Open_Menu(FarmTile farmTile)
    {
        // detecting which tile was pressed
        controller.Set_OpenTileNum(farmTile);

        // reset 
        controller.Reset_All_Menu();
        controller.Reset_All_Tile_Highlights();
        
        // highlight pressed tile
        Highlight_Tile();

        if (!seedPlanted)
        {
            controller.unPlantedMenu.Open();
        }
        else if (seedPlanted)
        {
            controller.plantedMenu.Open();
        }
    }
}
