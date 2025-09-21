using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableTop : MonoBehaviour
{
    [Space(20)] 
    [SerializeField] private Vector2 _cardGridRange;
    
    [Space(20)]
    [SerializeField] private CardLauncher _cardLauncher;
    [SerializeField] private Transform _launchedCards;

    [Space(20)]
    [SerializeField] private Vector2 _horizontalLaunchRange;
    [SerializeField] private Vector2 _verticalLaunchRange;

    [Space(20)] 
    [SerializeField] [Range(0, 100)] private int _startingCardAmount;
    
    
    private List<Card> _currentCards = new();
    public List<Card> currentCards => _currentCards;
    
    
    // MonoBehaviour
    private void Start()
    {
        StartCoroutine(LaunchCards_Coroutine());
        
        // subscriptions
    }

    private void OnDestroy()
    {
        // subscriptions
    }


    // New Cards
    private void Launch_StaticCard()
    {
        float xPos = UnityEngine.Random.Range(_horizontalLaunchRange.x, _horizontalLaunchRange.y);
        float yPos = UnityEngine.Random.Range(_verticalLaunchRange.x, _verticalLaunchRange.y);
        
        _cardLauncher.Launch_Card(new Vector2(xPos, yPos)).transform.SetParent(_launchedCards);
    }

    private IEnumerator LaunchCards_Coroutine()
    {
        for (int i = 0; i < _startingCardAmount; i++)
        {
            Launch_StaticCard();
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    
    // Current Cards
    public int Max_CardLayerOrder()
    {
        int maxOrder = 0;

        for (int i = 0; i < _currentCards.Count; i++)
        {
            int sortingOrder = _currentCards[i].sortingGroup.sortingOrder;
            if (sortingOrder <= maxOrder) continue;
            
            maxOrder = sortingOrder;
        }
        return maxOrder;
    }

    public bool Position_CardDropped(Vector2 checkPosition)
    {
        float threshold = 0.05f;

        for (int i = 0; i < _currentCards.Count; i++)
        {
            float distance = Vector2.Distance(checkPosition, _currentCards[i].transform.position);
            if (distance <= threshold) return true;
        }
        return false;
    }
}
