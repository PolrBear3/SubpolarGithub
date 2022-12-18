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
                // get rid of cloudy stun icon
                farmtile.statusIconIndicator.UnAssign_Status(2);
                // give back 1 day passed points
                farmtile.tileSeedStatus.dayPassed += 1;
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
    public void Scarecrow(FarmTile farmtile)
    {
        // current tile scarecrow buff
        var x = farmtile.statusIconIndicator.statusIcons;

        for (int i = 0; i < x.Length; i++)
        {
            if (x[i].hasStatus)
            {
                // if tile is crow attacked or crow additional attacked
                if (x[i].currentStatus.statusID == 4 || x[i].currentStatus.statusID == 5)
                {
                    // remove from farmtile active buff list
                    farmtile.Remove_Buff(functionController.controller.ID_Buff_Search(3));

                    // bring back to last state
                    farmtile.tileSeedStatus.health = farmtile.lastSeedStatus.health;
                    farmtile.tileSeedStatus.dayPassed = farmtile.lastSeedStatus.dayPassed;
                    farmtile.tileSeedStatus.watered = farmtile.lastSeedStatus.watered;
                    farmtile.tileSeedStatus.bonusPoints = farmtile.lastSeedStatus.bonusPoints;

                    // update tile
                    farmtile.TileSprite_Update_Check();
                }

                // if tile is crow attacked
                if (x[i].currentStatus.statusID == 4)
                {
                    // crow attacked icon
                    farmtile.statusIconIndicator.UnAssign_Status(4);
                    i = 0;
                }
                // if tile is additional crow attacked
                else if (x[i].currentStatus.statusID == 5)
                {
                    // delete additional crow attacked icon
                    farmtile.statusIconIndicator.UnAssign_Status(5);
                    i = 0;
                }
            }
            else break;
        }

        // surrounding tile scarecrow buff
        var y = functionController.controller.crossSurroundingFarmTiles;

        for (int i = 0; i < y.Count; i++)
        {
            for (int j = 0; j < y[i].statusIconIndicator.statusIcons.Length; j++)
            {
                if (y[i].statusIconIndicator.statusIcons[j].hasStatus)
                {
                    if (y[i].statusIconIndicator.statusIcons[j].currentStatus.statusID == 5)
                    {
                        y[i].tileSeedStatus.health = y[i].lastSeedStatus.health;
                        y[i].tileSeedStatus.dayPassed = y[i].lastSeedStatus.dayPassed;
                        y[i].tileSeedStatus.watered = y[i].lastSeedStatus.watered;

                        y[i].statusIconIndicator.UnAssign_Status(5);
                    }
                }
                else break;
            }
        }
    }
}
