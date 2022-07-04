using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainGame_Controller : MonoBehaviour
{
    public Default_Menu defaultMenu;
    public Locked_Menu lockedMenu;
    public UnPlanted_Menu unPlantedMenu;
    public Planted_Menu plantedMenu;
    public Time_System timeSystem;
    public Event_System eventSystem;

    public FarmTile[] farmTiles;
    public int openedTileNum = 0;
    
    private int _money = 0;
    public int money => _money;

    private void Start()
    {
        Lock_All_Tiles();
    }

    // tile functions
    public void Set_OpenTileNum(FarmTile farmTile)
    {
        openedTileNum = farmTile.tileNum;
    }
    private void Lock_All_Tiles()
    {
        for (int i = 0; i < farmTiles.Length; i++)
        {
            farmTiles[i].Lock_Tile();
        }
    }

    // ui functions
    public void Reset_All_Menu()
    {
        unPlantedMenu.Close();
        plantedMenu.Close();
        lockedMenu.Close();
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
        // from default menu, tween animation +cash moving fade 
        _money += amount;
        defaultMenu.Money_Update_Fade_Tween(true, amount);
        defaultMenu.Money_Text_Update();
    }
    public void Subtract_Money(int amount)
    {
        _money -= amount;
        defaultMenu.Money_Update_Fade_Tween(false, amount);
        defaultMenu.Money_Text_Update();
    }
}
