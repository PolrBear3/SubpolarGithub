using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    [ES3Serializable] private List<FoodData> _foodDatas = new();
    public List<FoodData> foodDatas => _foodDatas;
}
