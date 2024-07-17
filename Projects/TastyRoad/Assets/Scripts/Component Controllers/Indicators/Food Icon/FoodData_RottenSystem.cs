using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodData_RottenSystem : MonoBehaviour
{
    [SerializeField] private FoodData_Controller _foodIcon;

    [Header("")]
    [Range (0, 100)]
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
        if (_decayCount <= 0) return;
        if (_foodIcon.hasFood == false) return;

        // update count calculation
        int updateCount = _foodIcon.currentData.tikCount / _decayCount;

        // check if update is required
        if (updateCount <= 0) return;

        // reset rotten data
        _foodIcon.currentData.Clear_Condition(FoodCondition_Type.rotten);

        // set rotten data 
        for (int i = 0; i < updateCount; i++)
        {
            _foodIcon.currentData.Update_Condition(new FoodCondition_Data(FoodCondition_Type.rotten));
        }

        //
        _foodIcon.Show_Condition();
    }
}
