using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodList_Controller : MonoBehaviour
{
    [SerializeField] private Food_ScrObj[] _mergeFoods;
    public Food_ScrObj[] mergeFoods => _mergeFoods;
    
    
    // Gets
    public Food_ScrObj MergedFood(List<FoodData> ingredientDatas)
    {
        Data_Controller data = Main_Controller.instance.dataController;
        List<Food_ScrObj> mergableFoods = data.AllFoods(ingredientDatas);

        for (int i = 0; i < mergableFoods.Count; i++)
        {
            for (int j = 0; j < _mergeFoods.Length; j++)
            {
                if (mergableFoods[i] != _mergeFoods[j]) continue;
                return mergableFoods[i];
            }
        }
        return null;
    }
}
