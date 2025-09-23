using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableTop : MonoBehaviour
{
    [Space(20)] 
    [SerializeField] private Vector2 _cardGridRange;
    
    [SerializeField][Range(0, 10)] private float _cardSeperationDistance;
    public float cardSeperationDistance => _cardSeperationDistance;

    [Space(20)]
    [SerializeField] private CardLauncher _cardLauncher;
    [SerializeField] private Transform _launchedCards;

    [Space(20)] 
    [SerializeField][Range(0, 100)] private int _startingCardAmount;
    
    
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
        float randXPos = UnityEngine.Random.Range(-_cardGridRange.x, _cardGridRange.x);
        float randYPos = UnityEngine.Random.Range(-_cardGridRange.y, _cardGridRange.y);

        Card launchedCard = _cardLauncher.Launch_Card(new(randXPos, randYPos));
        launchedCard.transform.SetParent(_launchedCards);
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
}
