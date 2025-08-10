using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPC_FoodInteractionData
{
    private FoodData _orderFoodData; // order food + order count
    public FoodData orderFoodData => _orderFoodData;
    
    private int _serveCount;
    public int serveCount => _serveCount;
    
    private int _goldCount;
    public int goldCount => _goldCount;
    
    
    // New
    public NPC_FoodInteractionData(Food_ScrObj orderFood)
    {
        _orderFoodData = new(orderFood);
    }
    
    
    // Data
    public void Update_ServeCount(int goldValue)
    {
        _serveCount++;
        _goldCount += goldValue;
    }
}
