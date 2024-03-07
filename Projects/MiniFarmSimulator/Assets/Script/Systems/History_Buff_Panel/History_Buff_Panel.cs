using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class History_Buff_Panel : MonoBehaviour
{
    [SerializeField] private History_Buff_Icon[] icons;

    private void Clear_All()
    {
        for (int i = 0; i < icons.Length; i++)
        {
            icons[i].Clear_Icon();
        }
    }
    public void Assign_All(FarmTile farmTile)
    {
        Clear_All();

        var allBuffs = farmTile.currentBuffs;

        for (int i = 0; i < allBuffs.Count; i++)
        {
            if (allBuffs[i] == null) break;

            icons[i].Assign_Status(allBuffs[i]);
        }
    }
}