using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardData
{
    private CardScrObj _scrObj;
    public CardScrObj scrObj => _scrObj;


    // Constructors
    public CardData(CardScrObj cardScrObj)
    {
        _scrObj = cardScrObj;
    }
}
