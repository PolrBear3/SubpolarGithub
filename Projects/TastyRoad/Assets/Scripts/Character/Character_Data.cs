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
        _hungerLevel = Mathf.Round(Random.Range(0f, 100f));
        _inPatienceLevel = Mathf.Round(Random.Range(0f, 100f));
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