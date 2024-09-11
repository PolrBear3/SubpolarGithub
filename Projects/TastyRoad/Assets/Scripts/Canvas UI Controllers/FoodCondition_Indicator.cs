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


    //
    public void Indicate(FoodData ingredient)
    {
        Food_ScrObj ingredientFood = ingredient.foodScrObj;

        // food icon
        _foodIcon.sprite = ingredientFood.sprite;
        _foodIcon.rectTransform.anchoredPosition = ingredientFood.uiCenterPosition;

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
