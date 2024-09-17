using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Data : MonoBehaviour
{
    // attract
    private float _hungerLevel;
    public float hungerLevel => _hungerLevel;

    // clock speed
    private float _patienceLevel;
    public float patienceLevel => _patienceLevel;

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
        _patienceLevel = inPatientInt;
        _generosityLevel = generosityInt;
    }


    // data update
    public void Update_Hunger(float updateLevel)
    {
        // float previousLevel = _hungerLevel;

        _hungerLevel += updateLevel;
        Mathf.Clamp(_hungerLevel, 0, 100);

        // Debug.Log("hungerLevel level " + previousLevel + " => " + _hungerLevel);
    }

    public void Update_Patiency(float updateLevel)
    {
        // float previousLevel = _patienceLevel;

        _patienceLevel += updateLevel;
        Mathf.Clamp(_patienceLevel, 0, 100);

        // Debug.Log("patienceLevel level " + previousLevel + " => " + _patienceLevel);
    }

    public void Update_Generosity(float updateLevel)
    {
        // float previousLevel = _generosityLevel;

        _generosityLevel += updateLevel;
        Mathf.Clamp(_generosityLevel, 0, 100);

        // Debug.Log("generosity level " + previousLevel + " => " + _generosityLevel);
    }
}