using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableTop : MonoBehaviour
{
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
    }


    // Main
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
    
    
    // Visual
    public void Update_LayerOrders(Card placedCard)
    {
        for (int i = 0; i < _currentCards.Count; i++)
        {
            if (_currentCards[i] == placedCard) continue;
            _currentCards[i].sortingGroup.sortingOrder = 0;
        }
    }
}
