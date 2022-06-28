using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGame_Controller : MonoBehaviour
{
    public UnPlanted_Menu unPlantMenu;

    public FarmTile[] farmTiles;
    public int openedTileNum = 0;

    private void Start()
    {
        Lock_All_Tile_Row();
    }

    public void Set_OpenTileNum(FarmTile farmTile)
    {
        openedTileNum = farmTile.tileNum;
    }

    public void Lock_All_Tile_Row()
    {
        for (int i = 0; i < farmTiles.Length; i++)
        {
            farmTiles[i].Lock_Tile();
        }
    }

    public void Unlock_TileRow1()
    {
        for (int i = 0; i < 5; i++)
        {
            farmTiles[i].Unlock_Tile();
        }
    }
}
