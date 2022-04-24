using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public Default_Info info;
    public int currentAmount;

    // Item information connection with current amount
    public Item(Default_Info info, int currentAmount)
    {
        this.info = info;
        this.currentAmount = currentAmount;
    }

    public void Increase_Amount(int increaseAmount)
    {
        currentAmount += increaseAmount;
    }

    public void Decrease_Amount(int decreaseAmount)
    {
        currentAmount -= decreaseAmount;
    }
}
