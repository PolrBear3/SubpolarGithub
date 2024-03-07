using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status_Icon_Indicator : MonoBehaviour
{
    [SerializeField] private Planted_Menu plantedMenu;

    public Status_Icon[] statusIcons;
    private int currentIconNum;

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

        // if there is at least 1 current status amount
        if (statusAmount == 0) return;

        for (int i = 0; i < statusAmount; i++)
        {
            // if status icons are not full
            if (statusAmount == statusIcons.Length) break;

            statusIcons[i].Assign_Icon(currentFarmTile.currentStatuses[i]);
        }
    }

    public void Hide_Status_ToolTip()
    {
        currentIconNum = -1;
        plantedMenu.controller.statusToolTip.Hide();
    }
    public void Show_Hide_Status_ToolTip(int iconNum)
    {
        var statusToolTip = plantedMenu.controller.statusToolTip;
        
        // if the icon has a current status
        if (!statusIcons[iconNum].hasStatus) return;

        // if the status tooltip is on and the icon is pressed again, turn off status tooltip
        if (currentIconNum == iconNum)
        {
            currentIconNum = -1;
            statusToolTip.Hide();
        }
        // status tooltip on
        else
        {
            currentIconNum = iconNum;
            statusToolTip.Show(statusIcons[iconNum].currentStatus);
        }
    }
}
