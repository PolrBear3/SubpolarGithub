using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Detection : MonoBehaviour
{
    [Space(20)] 
    [SerializeField] private Card _card;

    [SerializeField] private BoxCollider2D _collider;
    public BoxCollider2D collider => _collider;

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
    public List<Card> Closest_DetectedCards()
    {
        List<Card> sortedCards = new(_detectedCards);
        Vector2 currentPos = _card.transform.position;
        
        sortedCards.Sort((a, b) =>
        {
            float distA = Vector2.Distance(a.transform.position, currentPos);
            float distB = Vector2.Distance(b.transform.position, currentPos);
            return distA.CompareTo(distB);
        });
        
        return sortedCards;
    }
    
    public Card Closest_DetectedCard()
    {
        if (_detectedCards.Count == 0) return null;
        Card closestCard = _detectedCards[0];

        for (int i = 0; i < _detectedCards.Count; i++)
        {
            if (i == 0) continue;
            if (_detectedCards[i] == null) continue;

            float closestDistance = Vector2.Distance(transform.position, closestCard.transform.position);
            if (Vector2.Distance(transform.position, _detectedCards[i].transform.position) >= closestDistance) continue;
            
            closestCard = _detectedCards[i];
        }
        
        return closestCard;
    }
}
