using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dispose_FoodDrop : ItemDrop
{
    [Space(20)]
    [SerializeField] private FoodData_Controller _dataController;


    // Main
    public void Set_FoodData(Food_ScrObj setFood)
    {
        _dataController.Set_CurrentData(new(setFood));
        _dataController.Show_Icon();
    }
}
