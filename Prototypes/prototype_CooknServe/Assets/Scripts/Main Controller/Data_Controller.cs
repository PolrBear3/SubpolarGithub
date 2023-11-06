using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    void Interact();
}


public class Data_Controller : MonoBehaviour
{
    public List<Food_ScrObj> allFoods = new();

    // Food
    public Food_ScrObj Get_Food(int foodID)
    {
        for (int i = 0; i < allFoods.Count; i++)
        {
            if (foodID != allFoods[i].id) continue;
            return allFoods[i];
        }
        return null;
    }
    public Food_ScrObj Get_Food(Food_ScrObj foodScrObj)
    {
        for (int i = 0; i < allFoods.Count; i++)
        {
            if (foodScrObj != allFoods[i]) continue;
            return allFoods[i];
        }
        return null;
    }
}