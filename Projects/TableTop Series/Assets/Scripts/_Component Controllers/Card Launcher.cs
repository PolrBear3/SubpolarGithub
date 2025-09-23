using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardLauncher : MonoBehaviour
{
    [Space(20)]
    [SerializeField] private GameObject _cardPrefab;
    
    
    // Main
    public Card Launch_Card(Vector2 spawnPosition)
    {
        if (_cardPrefab == null) return null;
        GameObject launchedCard = Instantiate(_cardPrefab, spawnPosition, Quaternion.identity);
        
        if (launchedCard.TryGetComponent(out Card card) == false) return null;
        
        // refresh detection
        BoxCollider2D collider = card.detection.boxCollider;
        
        collider.enabled = false;
        collider.enabled = true;
        
        return card;
    }
    
    // [] Launch Animation Effect
}
