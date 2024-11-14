using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodData_RottenSystem : MonoBehaviour
{
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
            // reset rotten data
            allDatas[i].Clear_Condition(FoodCondition_Type.rotten);

            // update count calculation
            int updateCount = allDatas[i].tikCount / _decayCount;

            // check if update is required
            if (updateCount <= 0) continue;

            // set rotten data 
            for (int j = 0; j < updateCount; j++)
            {
                allDatas[i].Update_Condition(new FoodCondition_Data(FoodCondition_Type.rotten));
            }
        }

        _foodIcon.Update_AllDatas(allDatas);
        _foodIcon.Show_Condition();
    }
}
