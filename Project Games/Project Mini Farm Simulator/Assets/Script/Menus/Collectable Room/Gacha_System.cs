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
                if (menu.allCollectables[i].currentAmount > 0)
                {
                    menu.allCollectables[i].unLocked = true;
                    break;
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
                menu.AllButton_Amount_Text_Update();

                // unlocking collectable
                Unlock_Collectable(menu.allCollectables[collectableNum].collectable);
                menu.UnlockCheck_CurrentButtonPage();

                // calculating money
                menu.controller.Subtract_Money(gachaPrice);
                menu.controller.defaultMenu.Money_Text_Update();
                menu.controller.defaultMenu.Money_Update_Fade_Tween(false, gachaPrice);
            }
        }
    }

    public void Gacha_Collectable()
    {
        // player has enough money
        if (menu.controller.money >= gachaPrice)
        {
            Gacha_Collectable_Calculation();
            menu.controller.shopMenu.Gacha_Animation_EventFunction();
        }
        // player does not have enough money
        else
        {
            menu.controller.defaultMenu.NotEnoughMoney_Blink_Tween();
        }
    }
}
