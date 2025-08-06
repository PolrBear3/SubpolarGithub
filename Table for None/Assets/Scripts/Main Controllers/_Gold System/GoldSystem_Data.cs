using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GoldSystem_Data
{
    [ES3Serializable] private int _goldAmount;
    public int goldAmount => _goldAmount;

    private int _bonusAddAmount;
    public int bonusAddAmount => _bonusAddAmount;
    
    private float _bonusMultiplyAmount;
    public float bonusMultiplyAmount => _bonusMultiplyAmount;


    // New
    public GoldSystem_Data(int goldAmount)
    {
        _goldAmount = goldAmount;
        _bonusMultiplyAmount = 1;
    }


    // Data Control
    public void Set_GoldAmount(int setValue)
    {
        _goldAmount = setValue;
    }

    
    public void Update_BonusAddAmount(int updateAmount)
    {
        _bonusAddAmount += updateAmount;
    }

    public void Set_BonusMultiplyAmount(float setAmount)
    {
        _bonusMultiplyAmount = setAmount;
    }
}