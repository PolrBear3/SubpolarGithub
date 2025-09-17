using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    private Card _currentCard;
    public Card currentCard => _currentCard;
    
    
    // MonoBehaviour
    private void Start()
    {
        
    }

    private void OnDestroy()
    {
        
    }


    // Main
    public void Set_CurrentCard(Card setCard)
    {
        _currentCard = setCard;
    }
}
