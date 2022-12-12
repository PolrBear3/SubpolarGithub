using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_Functions : MonoBehaviour
{
    public Buff_Function_Controller functionController;

    // all buff functions (goes through at function controller)
    public void Cloudy_Stun_Shield(FarmTile farmtile)
    {
        var x = farmtile.statusIconIndicator.statusIcons;
        
        for (int i = 0; i < x.Length; i++)
        {
            // if the farmtile is cloudy stunned
            if (x[i].hasStatus && x[i].currentStatus.statusID == 2)
            {
                // give back 1 day passed points
                farmtile.tileSeedStatus.dayPassed += 1;
                // get rid of cloudy stun icon
                farmtile.statusIconIndicator.UnAssign_Status(2);
                // update tile
                farmtile.TileSprite_Update_Check();
                functionController.controller.plantedMenu.Seed_Information_Update();
                // remove from farmtile active buff list
                farmtile.Remove_Buff(functionController.controller.ID_Buff_Search(0));
                // use single amount if there are multiple
                break;
            }
        }
    }
    public void Wait_Time_Decreaser(FarmTile farmtile)
    {
        var x = farmtile.currentBuffs;

        for (int i = 0; i < x.Count; i++)
        {
            if (x[i] == functionController.controller.ID_Buff_Search(1))
            {
                functionController.controller.timeSystem.mySec -= 5;

                farmtile.Remove_Buff(functionController.controller.ID_Buff_Search(1));
            }
        }
    }
    public void Golden_Sunny(FarmTile farmtile)
    {
        var x = farmtile.statusIconIndicator.statusIcons;

        for (int i = 0; i < x.Length; i++)
        {
            if (x[i].currentStatus == functionController.controller.ID_Status_Search(1))
            {
                // give +1 extra watered and dayPassed int
                farmtile.tileSeedStatus.watered += 1;
                farmtile.tileSeedStatus.dayPassed += 1;

                // if the tile is not full grown, addup +n bonus int
                if (farmtile.tileSeedStatus.dayPassed < farmtile.tileSeedStatus.fullGrownDay)
                {
                    farmtile.tileSeedStatus.bonusPoints += 1;
                }

                // if the seed's health is less than it's max health, give +n health int
                if (farmtile.tileSeedStatus.health < farmtile.data.plantedSeed.seedHealth)
                {
                    farmtile.tileSeedStatus.health += 1;
                }

                // update tile
                farmtile.TileSprite_Update_Check();
                functionController.controller.plantedMenu.Seed_Information_Update();

                // remove from farmtile active buff list
                farmtile.Remove_Buff(functionController.controller.ID_Buff_Search(2));
            }
        }
    }
}
