using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_Functions : MonoBehaviour
{
    public Buff_Function_Controller functionController;
    public Buff_ScrObj[] allBuffs;

    // all buff functions (goes through at function controller)
    public void Cloudy_Stun_Shield(FarmTile farmtile)
    {
        var x = farmtile.statusIconIndicator.statusIcons;
        
        for (int i = 0; i < x.Length; i++)
        {
            // if the farmtile is cloudy stunned
            if (x[i].hasStatus && x[i].currentStatus.statusType == StatusType.cloudyStunned)
            {
                // give back 1 day passed points
                farmtile.tileSeedStatus.dayPassed += 1;
                // get rid of cloudy stun icon
                farmtile.statusIconIndicator.UnAssign_Status(StatusType.cloudyStunned);
                // remove from farmtile buff list
                for (int j = 0; j < allBuffs.Length; j++)
                {
                    if (allBuffs[j].buffID == 0)
                    {
                        farmtile.Remove_Buff(allBuffs[j]);
                    }
                }
                // update tile
                farmtile.TileSprite_Update_Check();
                functionController.controller.plantedMenu.Seed_Information_Update();

                break;
            }
        }
    }
}
