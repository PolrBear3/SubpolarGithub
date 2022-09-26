using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_Functions : MonoBehaviour
{
    public Buff_Function_Controller functionController;
    
    public void Cloudy_Stun_Shield(FarmTile farmtile)
    {
        int removeAtNum = 0;

        // if the weather is cloudy
        if (functionController.controller.eventSystem.currentWeather.weatherID == 1)
        {
            // give back 1 day passed points
            farmtile.tileSeedStatus.dayPassed += 1;
            // get rid of cloudy stun icon
            farmtile.statusIconIndicator.UnAssign_Status(StatusType.cloudyStunned);
            // remove from buff list
            for (int i = 0; i < farmtile.currentBuffs.Count; i++)
            {
                if (farmtile.currentBuffs[i].buffID == 0)
                {
                    farmtile.currentBuffs.RemoveAt(removeAtNum);
                    break;
                }

                removeAtNum ++;
            }
            // update tile
            farmtile.TileSprite_Update_Check();
            functionController.controller.plantedMenu.Seed_Information_Update();
        }
    }
}
