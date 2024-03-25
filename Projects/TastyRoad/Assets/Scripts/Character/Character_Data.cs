using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Data : MonoBehaviour
{
    private float _hungerLevel;
    public float hungerLevel => _hungerLevel;

    private float _inPatienceLevel;
    public float inPatienceLevel => _inPatienceLevel;

    // UnityEngine
    private void Start()
    {
        Set_RandomData();
    }



    //
    private void Set_RandomData()
    {
        int hungerInt = Random.Range(0, 100);
        int inPatientInt = Random.Range(0, 100);

        _hungerLevel = hungerInt;
        _inPatienceLevel = inPatientInt;
    }



    // data update
    public void Update_Hunger(float updateLevel)
    {
        _hungerLevel += updateLevel;
    }

    public void Update_InPatiency(float updateLevel)
    {
        _inPatienceLevel += updateLevel;
    }
}