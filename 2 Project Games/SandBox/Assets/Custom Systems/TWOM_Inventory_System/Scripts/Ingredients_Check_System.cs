using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredients_Check_System : MonoBehaviour
{
    public Static_Slots_System kitchen;
    public Ingredients[] currentIngredients;

    public void CountAdd_Ingredients(Item_Info itemInfo, int amount)
    {
        for (int i = 0; i < currentIngredients.Length; i++)
        {
            if (itemInfo == currentIngredients[i].itemInfo)
            {
                currentIngredients[i].amount += amount;
            }
        }
    }
    public void CountSubtract_Ingredients(Item_Info itemInfo, int amount)
    {
        for (int i = 0; i < currentIngredients.Length; i++)
        {
            if (itemInfo == currentIngredients[i].itemInfo)
            {
                currentIngredients[i].amount -= amount;
            }
        }
    }

    private bool Current_Ingredients_Check(Item_Info ingredientInFood, int requiredAmount)
    {
        for (int i = 0; i < currentIngredients.Length; i++)
        {
            if (ingredientInFood == currentIngredients[i].itemInfo)
            {
                if (currentIngredients[i].amount >= requiredAmount)
                {
                    return true;
                }
                else return false;
            }
            else continue;
        }
        return false;
    }
    private bool Required_Ingredients_Check(Item_Info foodInfo)
    {
        int currentCheckAmount = 0;
        
        for (int i = 0; i < foodInfo.ingredients.Length; i++)
        {
            if (Current_Ingredients_Check(foodInfo.ingredients[i].itemInfo, foodInfo.ingredients[i].amount))
            {
                currentCheckAmount += 1;

                if (currentCheckAmount == foodInfo.ingredients.Length)
                {
                    return true;
                }
                else continue;
            }
            else continue;
        }
        return false;
    }
    
    public void Make_Food(Item_Info foodInfo)
    {
        if (Required_Ingredients_Check(foodInfo))
        {
            // take away ingredients from kitchen
            for (int i = 0; i < foodInfo.ingredients.Length; i++)
            {
                kitchen.Use_Item(foodInfo.ingredients[i].itemInfo, foodInfo.ingredients[i].amount);
            }

            kitchen.Craft_Item(foodInfo, 1);
        }
        else
        {
            Debug.Log("Not enough ingredients");
        }
    }
}
