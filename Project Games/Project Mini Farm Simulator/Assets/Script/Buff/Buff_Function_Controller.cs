using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_Function_Controller : MonoBehaviour
{
    public MainGame_Controller controller;

    // activate function
    public void Activate_All_Buffs_Seeds()
    {
        var x = controller.farmTiles;
        for (int i = 0; i < x.Length; i++)
        {
            // is the seed is planted
            if (x[i].data.seedPlanted)
            {
                GoThrough_All_Buffs(x[i]);
            }
        }
    }
    // activate buffs in single farm tile 
    private void GoThrough_All_Buffs(FarmTile farmtile)
    {
        var x = farmtile.currentBuffs;
        for (int i = 0; i < x.Count; i++)
        {

        }
    }

    // buffs
    private void Cloudy_Stun_Shield()
    {

    }
}
