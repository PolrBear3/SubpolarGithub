using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldSystem_Data
{
    [ES3Serializable] private int _goldAmount;
    public int goldAmount => _goldAmount;

    private int _bonusAddAmount;
    public int bonusAddAmount => _bonusAddAmount;


    // New
    public GoldSystem_Data(int goldAmount)
    {
        _goldAmount = goldAmount;
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
}