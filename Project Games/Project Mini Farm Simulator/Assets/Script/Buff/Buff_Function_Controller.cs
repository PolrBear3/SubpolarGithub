using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_Function_Controller : MonoBehaviour
{
    public MainGame_Controller controller;
    public Buff_Functions functions;

    // activate function (activates at time system Next Day function)
    public void Activate_All_Buffs_forSeeds()
    {
        var x = controller.farmTiles;
        for (int i = 0; i < x.Length; i++)
        {
            // if the seed is planted
            if (x[i].data.seedPlanted)
            {
                GoThrough_All_Buffs(x[i]);
            }
        }
    }
    
    // all cases 
    private void GoThrough_All_Buffs(FarmTile farmtile)
    {
        var x = farmtile.currentBuffs;
        for (int i = 0; i < x.Count; i++)
        {
            switch (x[i].buffID)
            {
                case 0:
                    functions.Cloudy_Stun_Shield(farmtile);
                    break;
                case 1:
                    functions.Wait_Time_Decreaser(farmtile);
                    break;
                case 2:
                    functions.Golden_Sunny(farmtile);
                    break;
                default:
                    break;
            }
        }
    }
}
