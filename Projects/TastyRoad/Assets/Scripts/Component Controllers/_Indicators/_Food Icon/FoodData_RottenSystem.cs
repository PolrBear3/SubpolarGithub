using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodData_RottenSystem : MonoBehaviour
{
    [Header("")]
    [SerializeField] private FoodData_Controller _foodIcon;

    [Header("")]
    [Range(0, 100)]
    [SerializeField] private int _decayCount;


    // UnityEngine
    private void Start()
    {
        _foodIcon.TimeTikEvent += Decay;
    }

    private void OnDestroy()
    {
        _foodIcon.TimeTikEvent -= Decay;
    }


    //
    private void Decay()
    {
        if (_foodIcon.hasFood == false) return;

        List<FoodData> allDatas = _foodIcon.AllDatas();

        for (int i = 0; i < allDatas.Count; i++)
        {
            int updateCount = allDatas[i].tikCount / _decayCount;
            int currentCount = allDatas[i].Current_ConditionLevel(FoodCondition_Type.rotten);

            if (updateCount <= currentCount) continue;

            allDatas[i].Update_Condition(new FoodCondition_Data(FoodCondition_Type.rotten, 1));
        }
        _foodIcon.Show_Condition();
    }
}
