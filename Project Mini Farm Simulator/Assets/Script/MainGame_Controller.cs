using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGame_Controller : MonoBehaviour
{
    public FarmTile[] farmTiles;
    public int openedTileNum = 0;
    // public GameObject farmTileMenu;

    void Set_OpenTileNum(FarmTile farmTile)
    {
        openedTileNum = farmTile.tileNum;
    }
    public void Open_Tile_Menu(FarmTile farmTile)
    {
        // farmTileMenu.Open_Tweet();
        
        Set_OpenTileNum(farmTile);
    }
}
