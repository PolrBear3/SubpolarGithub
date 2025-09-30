using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Data
{
    private Card_ScrObj _cardScrObj;
    public Card_ScrObj cardScrObj => _cardScrObj;

    private int _stackAmount;
    public int stackAmount => _stackAmount;
    
    
    // New
    public Card_Data(Card_ScrObj cardScrObj)
    {
        _cardScrObj = cardScrObj;
        _stackAmount = 1;
    }
    
    
    // Data
    public void Set_StackAmount(int setValue)
    {
        _stackAmount = (int)Mathf.Max(1f, setValue);
    }
}
