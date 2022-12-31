using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status_Icon_Indicator : MonoBehaviour
{
    [SerializeField] private Planted_Menu plantedMenu;

    public Status_Icon[] statusIcons;

    public void Reset_Status_Icons()
    {
        for (int i = 0; i < statusIcons.Length; i++)
        {
            statusIcons[i].Empty_Icon();
        }
    }

    public void Update_CurrentFarmTile_Status()
    {
        var currentFarmTileNum = plantedMenu.controller.openedTileNum;
        var currentFarmTile = plantedMenu.controller.farmTiles[currentFarmTileNum];
        int statusAmount = currentFarmTile.currentStatuses.Count;

        if (statusAmount != 0)
        {
            for (int i = 0; i < statusAmount; i++)
            {
                statusIcons[i].Assign_Icon(currentFarmTile.currentStatuses[i]);
            }
        }
    }
}
