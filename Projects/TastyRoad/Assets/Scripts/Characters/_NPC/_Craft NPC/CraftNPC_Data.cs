using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftNPC_Data
{
    [ES3Serializable] private int _price;
    public int price => _price;

    [ES3Serializable] private bool _payed;
    public bool payed => _payed;


    // New
    public CraftNPC_Data(CraftNPC_Data data)
    {
        _price = data._price;
        _payed = data._payed;
    }

    public CraftNPC_Data(int price)
    {
        _price = price;
        _payed = false;
    }

    public CraftNPC_Data(bool payed)
    {
        _payed = payed;
    }
}
