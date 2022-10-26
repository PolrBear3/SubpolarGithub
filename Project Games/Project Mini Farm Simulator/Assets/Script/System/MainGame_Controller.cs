using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainGame_Controller : MonoBehaviour
{
    public Default_Menu defaultMenu;
    public CollectableRoom_Menu collectableRoomMenu;
    public Locked_Menu lockedMenu;
    public UnPlanted_Menu unPlantedMenu;
    public Planted_Menu plantedMenu;
    public Buff_Menu buffMenu;
    public Shop_Menu shopMenu;
    public Buff_Function_Controller buffFunction;
    public Gacha_System gachaSystem;
    public Save_System saveSystem;
    public Time_System timeSystem;
    public Event_System eventSystem;

    public Season_ScrObj[] allSeasons;
    public Weather_ScrObj[] allWeathers;
    public Seed_ScrObj[] allSeeds;
    public Buff_ScrObj[] allBuffs;
    public Status[] allStatus;

    public FarmTile[] farmTiles;
    public int openedTileNum = 0;
    
    private int _money = 0;
    public int money => _money;

    private void Start()
    {
        Set_TileNum_forAll_Tiles();
    }

    // tile functions
    private void Set_TileNum_forAll_Tiles()
    {
        int tileNum = 0;
        for (int i = 0; i < farmTiles.Length; i++)
        {
            farmTiles[i].data.tileNum = tileNum;
            tileNum++;
        }
    }
    public void Set_OpenTileNum(FarmTile farmTile)
    {
        openedTileNum = farmTile.data.tileNum;
    }

    // ui functions
    public void Reset_All_Menu()
    {
        unPlantedMenu.Close();
        plantedMenu.Close();
        lockedMenu.Close();
        buffMenu.Close();
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
        defaultMenu.Money_Update_Fade_Tween(true, amount);
        defaultMenu.Money_Text_Update();
        defaultMenu.AddMoney_Blink();
    }
    public void Add_Money_withBonus(int originalAmount, int bonusAmount)
    {
        _money += originalAmount; 
        _money += bonusAmount;
        defaultMenu.Money_withBonus_Update_Fade_Tween(originalAmount, bonusAmount);
        defaultMenu.Money_Text_Update();
        defaultMenu.AddMoney_Blink();
    }
    public void Subtract_Money(int amount)
    {
        _money -= amount;
        defaultMenu.Money_Update_Fade_Tween(false, amount);
        defaultMenu.Money_Text_Update();
        defaultMenu.SubtractMoney_RedBlink();
    }
    public void Load_Money(int amount)
    {
        _money = amount;
    }
}
