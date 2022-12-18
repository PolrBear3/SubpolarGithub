using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGame_Controller : MonoBehaviour
{
    public Default_Menu defaultMenu;
    public CollectableRoom_Menu collectableRoomMenu;
    public TileBuy_Button tileBuyButton;
    public UnPlanted_Menu unPlantedMenu;
    public Planted_Menu plantedMenu;
    public Buff_Menu buffMenu;
    public Shop_Menu shopMenu;
    public Options_Menu optionsMenu;
    public Buff_Function_Controller buffFunction;
    public Save_System saveSystem;
    public Time_System timeSystem;
    public Event_System eventSystem;
    public Gacha_System gachaSystem;

    public FarmTile[] farmTiles;
    [HideInInspector]
    public int openedTileNum = 0;
    [HideInInspector]
    public List<FarmTile> crossSurroundingFarmTiles = new List<FarmTile>();

    private int _money = 0;
    public int money => _money;

    public Season_ScrObj[] allSeasons;
    public Weather_ScrObj[] allWeathers;
    public Seed_ScrObj[] allSeeds;
    public Buff_ScrObj[] allBuffs;
    public Status[] allStatus;

    private void Start()
    {
        Set_TileNum_forAll_Tiles();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Add_Money(100);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            for (int i = 0; i < farmTiles.Length; i++)
            {
                if (farmTiles[i].data.seedPlanted && !farmTiles[i].tileSeedStatus.currentDayWatered)
                {
                    // water the seeded tile
                    farmTiles[i].tileSeedStatus.currentDayWatered = true;
                    farmTiles[i].tileSeedStatus.watered += 1;
                    farmTiles[i].statusIconIndicator.Assign_Status(0);
                    farmTiles[i].Watering_Check();
                }
            }
        }
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
    public void Reset_All_Tile_Highlights()
    {
        for (int i = 0; i < farmTiles.Length; i++)
        {
            farmTiles[i].UnHighlight_Tile();
        }
    }
    public void Hide_All_Tiles(bool activation)
    {
        bool x;
        
        if (activation)
        {
            x = false;
        }
        else
        {
            x = true;
        }

        for (int i = 0; i < farmTiles.Length; i++)
        {
            farmTiles[i].statusIconIndicator.gameObject.SetActive(x);
            farmTiles[i].button.enabled = x;
        }
    }

    public void Set_Cross_Surrounding_FarmTiles(int tileNum)
    {
        crossSurroundingFarmTiles.Clear();

        // top tile
        if (tileNum - 5 >= 0)
        {
            crossSurroundingFarmTiles.Add(farmTiles[tileNum - 5]);
        }
        // bottom tile
        if (tileNum + 5 <= 24)
        {
            crossSurroundingFarmTiles.Add(farmTiles[tileNum + 5]);
        }
        // left tile
        if (tileNum - 1 >= 0)
        {
            crossSurroundingFarmTiles.Add(farmTiles[tileNum - 1]);
        }
        // right tile
        if (tileNum + 1 <= 24)
        {
            crossSurroundingFarmTiles.Add(farmTiles[tileNum + 1]);
        }
    }

    // ID Search
    public Buff_ScrObj ID_Buff_Search(int buffID)
    {
        for (int i = 0; i < allBuffs.Length; i++)
        {
            if (buffID == allBuffs[i].buffID)
            {
                return allBuffs[i];
            }
        }
        return null;
    }
    public Status ID_Status_Search(int statusID)
    {
        for (int i = 0; i < allStatus.Length; i++)
        {
            if (statusID == allStatus[i].statusID)
            {
                return allStatus[i];
            }
        }
        return null;
    }

    // ui functions
    public void Reset_All_Menu()
    {
        plantedMenu.Close();
        unPlantedMenu.Close();
        tileBuyButton.Close();
        buffMenu.Close();
        shopMenu.Close();
        collectableRoomMenu.Close();
        optionsMenu.Close();
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