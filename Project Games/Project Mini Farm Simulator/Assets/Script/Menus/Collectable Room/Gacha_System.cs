using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gacha_System : MonoBehaviour
{
    public CollectableRoom_Menu menu;

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
    public void Gacha_Random_Collectable()
    {
        bool passed = false;
        while (!passed)
        {
            int collectableNum = Random.Range(0, menu.allCollectables.Length);

            if (Check_Selected_Collectable(menu.allCollectables[collectableNum].collectable))
            {
                passed = true;
                menu.allCollectables[collectableNum].currentAmount += 1;
            }
        }
    }
}
