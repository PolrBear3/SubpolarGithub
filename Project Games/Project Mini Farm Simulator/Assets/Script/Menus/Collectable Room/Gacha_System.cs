using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gacha_System : MonoBehaviour
{
    public CollectableRoom_Menu menu;
    public int gachaPrice;

    private bool Percentage_Setter(float percentage)
    {
        var value = (100 - percentage) * 0.01f;
        if (Random.value > value)
        {
            return true;
        }
        else return false;
    }

    private bool Check_Selected_Collectable(Collectable_ScrObj checkCollectable)
    {
        for (int i = 0; i < menu.allCollectableTierData.Length; i++)
        {
            if (checkCollectable.colorLevel == menu.allCollectableTierData[i].colorLevel)
            {
                if (Percentage_Setter(menu.allCollectableTierData[i].tierPercentage))
                {
                    return true;
                }
            }
        }

        return false;
    }
    private void Unlock_Collectable(Collectable_ScrObj collectable)
    {
        for (int i = 0; i < menu.allCollectables.Length; i++)
        {
            if (collectable == menu.allCollectables[i].collectable)
            {
                if (menu.allCollectables[i].currentAmount > 0 && !menu.allCollectables[i].unLocked)
                {
                    menu.allCollectables[i].unLocked = true;
                    break;
                }
            }
        }
    }
    private void GoldMode_Collectable_Check(Collectable_ScrObj collectable)
    {
        for (int i = 0; i < menu.allCollectables.Length; i++)
        {
            if (collectable == menu.allCollectables[i].collectable)
            {
                if (menu.allCollectables[i].maxAmount >= 100)
                {
                    menu.allCollectables[i].goldModeAvailable = true;
                    menu.allCollectables[i].currentAmount = 100;
                }
            }
        }
    }
    private void Gacha_Collectable_Calculation()
    {
        bool passed = false;
        while (!passed)
        {
            int collectableNum = Random.Range(0, menu.allCollectables.Length);

            if (Check_Selected_Collectable(menu.allCollectables[collectableNum].collectable))
            {
                passed = true;

                // adding collectable amount
                menu.allCollectables[collectableNum].currentAmount += 1;
                menu.allCollectables[collectableNum].maxAmount += 1;

                // send collectable information to shop menu for animation
                menu.controller.shopMenu.data.lastWinCollectable = menu.allCollectables[collectableNum].collectable;

                // unlocking and update collectables
                Unlock_Collectable(menu.allCollectables[collectableNum].collectable);
                GoldMode_Collectable_Check(menu.allCollectables[collectableNum].collectable);
                menu.AllButton_UnlockCheck();
                menu.AllButton_NewIcon_Check();
                menu.AllButton_GoldMode_Check();
                menu.AllButton_Amount_Text_Update();

                // calculating money
                menu.controller.Subtract_Money(gachaPrice);
                menu.controller.defaultMenu.Money_Text_Update();
                menu.controller.defaultMenu.Money_Update_Fade_Tween(false, gachaPrice, 0);
            }
        }
    }

    public void Gacha_Collectable()
    {
        // player has enough money
        if (menu.controller.money >= gachaPrice)
        {
            Gacha_Collectable_Calculation();
            // roll stop win anim, gacha buy button unavailable, go back to roll after x time
            menu.controller.shopMenu.ui.anim.SetBool("gachaPressed", true);
        }
        // player does not have enough money
        else
        {
            menu.controller.defaultMenu.NotEnoughMoney_Blink_Tween();
        }
    }

    // sound
    public void Play_Sound_onSelect(AudioClip clip)
    {
        // player has not enough money
        if (menu.controller.money < gachaPrice) return;
        
        menu.controller.soundController.Play_SFX(clip);
    }
}
