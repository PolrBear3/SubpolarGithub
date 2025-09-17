using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Movement : MonoBehaviour
{
    [Space(20)] 
    [SerializeField] private IPointer_EventSystem _eventSystem;
    
    [Space(20)] 
    [SerializeField] private SpriteRenderer _cardSprite;
    [SerializeField] private SpriteRenderer _cardShadow;
    
    
    // MonoBehaviour
    private void Start()
    {
        // subscriptions
        _eventSystem.OnClick += Lift;
    }

    private void OnDestroy()
    {
        // subscriptions
        _eventSystem.OnClick -= Lift;
    }


    // Main
    private void Lift()
    {
        
    }

    private void Update_LiftIndication()
    {
        
    }
}