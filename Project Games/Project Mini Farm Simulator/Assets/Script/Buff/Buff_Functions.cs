using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_Functions : MonoBehaviour
{
    public Buff_Function_Controller functionController;
    
    public void Cloudy_Stun_Shield(FarmTile farmtile)
    {
        int removeAtNum = 0;

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
                // remove from buff list
                for (int j = 0; j < farmtile.currentBuffs.Count; j++)
                {
                    if (farmtile.currentBuffs[j].buffID == 0)
                    {
                        farmtile.currentBuffs.RemoveAt(removeAtNum);
                        break;
                    }

                    removeAtNum++;
                }
                // update tile
                farmtile.TileSprite_Update_Check();
                functionController.controller.plantedMenu.Seed_Information_Update();

                break;
            }
        }
    }
}
