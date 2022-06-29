using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGame_Controller : MonoBehaviour
{
    public UnPlanted_Menu unPlantedMenu;
    public Planted_Menu plantedMenu;

    public FarmTile[] farmTiles;
    public int openedTileNum = 0;

    public void Set_OpenTileNum(FarmTile farmTile)
    {
        openedTileNum = farmTile.tileNum;
    }

    public void Reset_All_Menu()
    {
        unPlantedMenu.Close();
        plantedMenu.Close();
    }
    public void Reset_All_Tile_Highlights()
    {
        for (int i = 0; i < farmTiles.Length; i++)
        {
            farmTiles[i].UnHighlight_Tile();
        }
    }
}
