using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Data : MonoBehaviour
{
    // attract
    private float _hungerLevel;
    public float hungerLevel => _hungerLevel;

    // clock speed
    private float _inPatienceLevel;
    public float inPatienceLevel => _inPatienceLevel;

    // item drop
    private float _generosityLevel;
    public float generosityLevel => _generosityLevel;


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
        int generosityInt = Random.Range(0, 100);

        _hungerLevel = hungerInt;
        _inPatienceLevel = inPatientInt;
        _generosityLevel = generosityInt;
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

    public void Update_Generosity(float updateLevel)
    {
        _generosityLevel += updateLevel;
    }
}