using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodCondition_Indicator : MonoBehaviour
{
    [SerializeField] private FoodData_Controller _forConditionSprites;

    [Header("")]
    [SerializeField] private Image _foodIcon;
    [SerializeField] private Image[] _conditionBoxes;


    // Indications
    public void Clear()
    {
        _foodIcon.color = Color.clear;

        foreach (Image boxes in _conditionBoxes)
        {
            boxes.color = Color.clear;
        }
    }

    public void Indicate(FoodData ingredient)
    {
        Food_ScrObj ingredientFood = ingredient.foodScrObj;

        _foodIcon.sprite = ingredientFood.sprite;
        _foodIcon.color = Color.white;

        int conditionCount = ingredient.conditionDatas.Count;

        // condition boxes
        for (int i = 0; i < _conditionBoxes.Length; i++)
        {
            if (conditionCount <= 0)
            {
                _conditionBoxes[i].color = Color.clear;
                continue;
            }

            FoodCondition_Type type = ingredient.conditionDatas[i].type;
            int level = ingredient.conditionDatas[i].level;

            _conditionBoxes[i].color = Color.white;
            _conditionBoxes[i].sprite = _forConditionSprites.Get_ConditionSprites(type).sprites[level - 1];
            conditionCount--;
        }
    }
}
