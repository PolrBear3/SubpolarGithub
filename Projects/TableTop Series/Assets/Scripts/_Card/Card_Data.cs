using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Data
{
    private Card_ScrObj _cardScrObj;
    public Card_ScrObj cardScrObj => _cardScrObj;
    
    private List<Card_Data> _stackedCardDatas = new();
    public List<Card_Data> stackedCardDatas => _stackedCardDatas;
    
    
    // New
    public Card_Data(Card_ScrObj cardScrObj)
    {
        _cardScrObj = cardScrObj;
    }
}
