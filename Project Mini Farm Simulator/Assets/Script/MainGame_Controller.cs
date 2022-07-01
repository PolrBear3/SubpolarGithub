using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainGame_Controller : MonoBehaviour
{
    public Default_Menu defaultMenu;
    public UnPlanted_Menu unPlantedMenu;
    public Planted_Menu plantedMenu;
    public Time_System timeSystem;
    public Event_System eventSystem;

    public FarmTile[] farmTiles;
    public int openedTileNum = 0;
    
    private int _money = 0;
    public int money => _money;

    public void Set_OpenTileNum(FarmTile farmTile)
    {
        openedTileNum = farmTile.tileNum;
    }

    // ui functions
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

    // money functions
    public void Add_Money(int amount)
    {
        _money += amount;
        defaultMenu.Money_Text_Update();
    }
    public void Subtract_Money(int amount)
    {
        _money -= amount;
        defaultMenu.Money_Text_Update();
    }
}
