using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodData_RottenSystem : MonoBehaviour
{
    private FoodData_Controller _foodIcon;

    [SerializeField] private int _updateTikTime;



    // UnityEngine
    private void Awake()
    {
        _foodIcon = gameObject.GetComponent<FoodData_Controller>();
    }



    //
    public void UpdateDecay_Toggle(bool toggleOn)
    {
        if (toggleOn) GlobalTime_Controller.TimeTik_Update += Decay_TikTimeUpdate;
        else GlobalTime_Controller.TimeTik_Update -= Decay_TikTimeUpdate;
    }



    //
    private void Decay_TikTimeUpdate()
    {
        if (_foodIcon.currentFoodData.currentTikTime >= _updateTikTime)
        {
            _foodIcon.Update_State(FoodState_Type.rotten, 1);
            _foodIcon.currentFoodData.currentTikTime = 0;

            return;
        }

        _foodIcon.currentFoodData.currentTikTime++;
    }
}
