using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Save_System : MonoBehaviour
{
    public MainGame_Controller controller;

    private void Start()
    {
        LoadAll_Tiles();
        Load_Money();
    }
    private void OnApplicationQuit()
    {
        SaveAll_Tiles();
        Save_Money();
    }

    // farm tiles
    private void SaveAll_Tiles()
    {
        for (int i = 0; i < controller.farmTiles.Length; i++)
        {
            Save_FarmTile(controller.farmTiles[i]);
        }
    }
    private void Save_FarmTile(FarmTile farmTile)
    {
        // data
        ES3.Save("farmTile" + farmTile.saveName + " tileLocked", farmTile.data.tileLocked);
    }

    private void LoadAll_Tiles()
    {
        for (int i = 0; i < controller.farmTiles.Length; i++)
        {
            Load_FarmTile(controller.farmTiles[i]);
            controller.farmTiles[i].Load_Update_Tile();
        }
    }
    private void Load_FarmTile(FarmTile farmTile)
    {
        // data
        if (ES3.KeyExists("farmTile" + farmTile.saveName + " tileLocked"))
        {
            farmTile.data.tileLocked = ES3.Load<bool>("farmTile" + farmTile.saveName + " tileLocked");
        }
    }

    // money
    private void Save_Money()
    {
        ES3.Save("money", controller.money);
    }
    private void Load_Money()
    {
        if (ES3.KeyExists("money"))
        {
            controller.Load_Money(ES3.Load<int>("money"));
        }
    }

    // season, weather, day
}
