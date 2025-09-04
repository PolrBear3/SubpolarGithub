using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ArchiveMenu_Data : VehicleMenu_Data
{
    [ES3Serializable] private List<FoodData> _unlockedIngredients = new();
    public List<FoodData> unlockedIngredients => _unlockedIngredients;
}
