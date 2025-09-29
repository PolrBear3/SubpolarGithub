using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Detection : MonoBehaviour
{
    [Space(20)] 
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
    
    
    // Detected Cards
    public Card Closest_DetectedCard()
    {
        if (_detectedCards.Count == 0) return null;
        Card closestCard = _detectedCards[0];

        for (int i = 0; i < _detectedCards.Count; i++)
        {
            if (i == 0) continue;
            
            float closestDistance = Vector2.Distance(transform.position, closestCard.transform.position);
            if (Vector2.Distance(transform.position, _detectedCards[i].transform.position) >= closestDistance) continue;
            
            closestCard = _detectedCards[i];
        }
        
        return closestCard;
    }
}
