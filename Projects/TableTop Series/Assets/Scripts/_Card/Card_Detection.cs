using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Detection : MonoBehaviour
{
    [Space(20)] 
    [SerializeField] private BoxCollider2D _collider;
    public BoxCollider2D boxCollider => _collider;
    
    [SerializeField] private Card _card;

    [Space(20)] 
    [SerializeField] private List<Card> _detectedCards = new();
    public List<Card> detectedCards => _detectedCards;
    
    public Action OnCardDetection;
    public Action OnCardExit;
    

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
        
        OnCardExit?.Invoke();
    }
}
