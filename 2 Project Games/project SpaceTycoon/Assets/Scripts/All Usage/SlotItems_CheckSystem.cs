using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotItems_CheckSystem : MonoBehaviour
{
    public Guest_System guestSystem;
    public AllItem_CountTracker itemCounter;

    // use to add count 
    public void AddCount_Ingredients(Item_Info itemInfo, int amount)
    {
        for (int i = 0; i < itemCounter.allItems.Length; i++)
        {
            if (itemInfo == itemCounter.allItems[i].itemInfo)
            {
                itemCounter.allItems[i].amount += amount;
            }
        }
    }

    // use to subtract count
    public void SubtractCount_Ingredients(Item_Info itemInfo, int amount)
    {
        for (int i = 0; i < itemCounter.allItems.Length; i++)
        {
            if (itemInfo == itemCounter.allItems[i].itemInfo)
            {
                itemCounter.allItems[i].amount -= amount;
            }
        }
    }

    private bool Current_Ingredients_Check(Item_Info ingredientInItem, int requiredAmount)
    {
        for (int i = 0; i < itemCounter.allItems.Length; i++)
        {
            if (ingredientInItem == itemCounter.allItems[i].itemInfo)
            {
                if (itemCounter.allItems[i].amount >= requiredAmount)
                {
                    return true;
                }
                else return false;
            }
            else continue;
        }
        return false;
    }

    // use this for craft check
    public bool Required_Ingredients_Check(Item_Info itemInfo)
    {
        int currentCheckAmount = 0;

        for (int i = 0; i < itemInfo.ingredients.Length; i++)
        {
            if (Current_Ingredients_Check(itemInfo.ingredients[i].itemInfo, itemInfo.ingredients[i].amount))
            {
                currentCheckAmount += 1;

                if (currentCheckAmount == itemInfo.ingredients.Length)
                {
                    return true;
                }
                else continue;
            }
            else continue;
        }
        return false;
    }
}
