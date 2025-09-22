using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Detection : MonoBehaviour
{
    [Space(20)] 
    [SerializeField] private Card _card;

    
    [SerializeField] private List<Card> _detectedCards = new();
    public List<Card> detectedCards => _detectedCards;
    
    public Action OnCardDetection;
    

    // OnTrigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Card card) == false) return;
        _detectedCards.Add(card);
        
        OnCardDetection?.Invoke();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out Card card) == false) return;
        _detectedCards.Remove(card);
    }
}
