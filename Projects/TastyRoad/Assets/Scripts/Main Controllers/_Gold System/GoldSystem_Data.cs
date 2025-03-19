using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldSystem_Data
{
    [ES3Serializable] private int _goldAmount;
    public int goldAmount => _goldAmount;


    public GoldSystem_Data(int goldAmount)
    {
        _goldAmount = goldAmount;
    }


    public void Set_GoldAmount(int setValue)
    {
        _goldAmount = setValue;
    }
}