using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodData_RottenSystem : MonoBehaviour
{
    private FoodData_Controller _foodIcon;

    [SerializeField] private float _updateIntervalTime;

    private Coroutine _decayCoroutine;



    // UnityEngine
    private void Awake()
    {
        _foodIcon = gameObject.GetComponent<FoodData_Controller>();
    }



    //
    public void StartDecay()
    {
        ResetDecay();

        _decayCoroutine = StartCoroutine(StartDecay_Coroutine());
    }
    private IEnumerator StartDecay_Coroutine()
    {
        FoodState_Data maxRottenData = new(FoodState_Type.rotten, 3);

        // waiting to get all game component on game load
        yield return new WaitForSeconds(_updateIntervalTime);

        while (_foodIcon.Has_StateData(maxRottenData) == false)
        {
            _foodIcon.Update_State(FoodState_Type.rotten, 1);

            yield return new WaitForSeconds(_updateIntervalTime);
        }
    }



    public void ResetDecay()
    {
        if (_decayCoroutine != null)
        {
            StopCoroutine(_decayCoroutine);
            _decayCoroutine = null;
        }
    }
}
