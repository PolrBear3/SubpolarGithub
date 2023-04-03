using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class History_Status_Panel : MonoBehaviour
{
    [SerializeField] private Death_Menu deathMenu;
    
    [SerializeField] private History_Status_Icon[] statusIcons;
    private int currentIconNum;

    private void Clear_All()
    {
        for (int i = 0; i < statusIcons.Length; i++)
        {
            statusIcons[i].Clear_Icon();
        }
    }
    public void Assign_All(FarmTile farmTile)
    {
        Clear_All();

        var allStatuses = farmTile.currentStatuses;

        for (int i = 0; i < allStatuses.Count; i++)
        {
            if (allStatuses[i] == null) break;

            statusIcons[i].Assign_Status(allStatuses[i]);
        }
    }

    public void Hide_Status_ToolTip()
    {
        currentIconNum = -1;
        deathMenu.controller.statusToolTip.Hide();
    }
    public void Show_Hide_Status_ToolTip(int iconNum)
    {
        var pressedIcon = statusIcons[iconNum];

        // if the icon has status
        if (!pressedIcon.hasStatus) return;

        var statusToolTip = deathMenu.controller.statusToolTip;

        // close tooltip if same icon is pressed again
        if (currentIconNum == iconNum)
        {
            Hide_Status_ToolTip();
        }
        // open tooltip
        else
        {
            // close seed tooltip
            deathMenu.controller.unPlantedMenu.Hide_Seed_ToolTip();

            currentIconNum = iconNum;
            statusToolTip.Show(pressedIcon.currentStatus);
        }
    }
}
