using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmTile_Menu : MonoBehaviour
{
    public MainGame_Controller controller;
    
    // Open_Tweet() {}

    public void Plant_Seed(Seed_ScrObj seedtype)
    {
        for (int i = 0; i < controller.farmTiles.Length; i++)
        {
            if (controller.openedTileNum == controller.farmTiles[i].tileNum)
            {
                controller.farmTiles[i].plantSeed = seedtype;
                controller.farmTiles[i].seedPlanted = true;
                break;
            }
        }
    }
}
