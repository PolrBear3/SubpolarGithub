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

    private void OnDestroy()
    {
        
    }



    //
    public void UpdateDecay_Toggle(bool toggleOn)
    {

    }



    //
    private void Decay_TikTimeUpdate()
    {
        /*
        if (_foodIcon.currentData.currentTikTime >= _updateTikTime)
        {
            _foodIcon.Update_State(FoodState_Type.rotten, 1);
            _foodIcon.currentData.currentTikTime = 0;

            return;
        }

        _foodIcon.currentData.currentTikTime++;
        */
    }
}
