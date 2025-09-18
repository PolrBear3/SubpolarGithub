using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardLauncher : MonoBehaviour
{
    [Space(20)]
    [SerializeField] private GameObject _cardPrefab;
    
    
    // Main
    public Card Launch_Card(Vector2 launchDestination)
    {
        if (_cardPrefab == null) return null;
        GameObject launchedCard = Instantiate(_cardPrefab, transform.position, Quaternion.identity);
        
        if (launchedCard.TryGetComponent(out Card card) == false) return null;
        card.movement.Assign_TargetPosition(launchDestination);
        
        return card;
    }
}
