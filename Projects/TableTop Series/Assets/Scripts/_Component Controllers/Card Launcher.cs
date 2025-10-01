using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardLauncher : MonoBehaviour
{
    [Space(20)]
    [SerializeField] private GameObject _cardPrefab;
    
    
    // Main
    public Card Launch_Card(Vector2 launchPosition, Vector2 launchDestination)
    {
        if (_cardPrefab == null) return null;
        GameObject launchedCard = Instantiate(_cardPrefab, launchPosition, Quaternion.identity);
        
        if (launchedCard.TryGetComponent(out Card card) == false) return null;
        card.sortingGroup.sortingOrder = 0;
        
        if (launchPosition == launchDestination) return card;
        card.movement.Assign_TargetPosition(launchDestination);
        
        return card;
    }
    public Card Launch_Card(Vector2 launchDestination)
    {
        return Launch_Card(transform.position, launchDestination);
    }
    
    
    // [] Launch Animation Effect
}
