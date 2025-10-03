using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Data
{
    private Card_ScrObj _cardScrObj;
    public Card_ScrObj cardScrObj => _cardScrObj;

    private Vector2 _droppedPosition;
    public Vector2 droppedPosition => _droppedPosition;
    
    
    // New
    public Card_Data(Card_ScrObj cardScrObj)
    {
        _cardScrObj = cardScrObj;
    }
    
    
    // Data
    public void Update_DroppedPosition(Vector2 position)
    {
        _droppedPosition = position;
    }
}
