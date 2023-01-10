using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class History_Status_Panel : MonoBehaviour
{
    public History_Status_Icon[] statusIcons;

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
}
