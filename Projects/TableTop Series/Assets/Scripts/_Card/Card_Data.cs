using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Card_Data
{
    private Card_ScrObj _cardScrObj;
    public Card_ScrObj cardScrObj => _cardScrObj;

    private Vector2 _droppedPosition;
    public Vector2 droppedPosition => _droppedPosition;

    private int _maxFillBarValue;
    public int maxFillBarValue => _maxFillBarValue;

    private int _currentFillBarValue;
    public int currentFillBarValue => _currentFillBarValue;


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


    public void SetMax_FillBarValue(int setValue)
    {
        _maxFillBarValue = Mathf.Max(0, setValue);
    }

    public void SetCurrent_FillBarValue(int setValue)
    {
        _currentFillBarValue = Mathf.Clamp(setValue, 0, _maxFillBarValue);
    }
}
