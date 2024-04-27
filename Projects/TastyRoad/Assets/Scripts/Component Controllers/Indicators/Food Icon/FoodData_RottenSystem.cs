using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodData_RottenSystem : MonoBehaviour
{
    private FoodData_Controller _foodIcon;

    [SerializeField] private int _updateTikCount;
    private int _currentTikCount;



    // UnityEngine
    private void Awake()
    {
        _foodIcon = gameObject.GetComponent<FoodData_Controller>();
    }

    private void Start()
    {
        _foodIcon.TimeTikEvent += Decay_TikTimeUpdate;
    }

    private void OnDestroy()
    {
        _foodIcon.TimeTikEvent -= Decay_TikTimeUpdate;
    }



    //
    private void Decay_TikTimeUpdate()
    {
        if (_foodIcon.hasFood == false) return;

        if (_currentTikCount >= _updateTikCount)
        {
            _foodIcon.currentData.Update_Condition(new FoodCondition_Data(FoodCondition_Type.rotten));
            _foodIcon.Show_Condition();

            _currentTikCount = 0;
            return;
        }

        _currentTikCount++;
    }
}
