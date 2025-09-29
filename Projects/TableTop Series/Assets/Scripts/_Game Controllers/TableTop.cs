using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableTop : MonoBehaviour
{
    [Space(20)] 
    [SerializeField] private Vector2 _xGridRange;
    [SerializeField] private Vector2 _yGridRange;

    [Space(10)]
    [SerializeField][Range(0, 100)] private float _cardSeperationDistance;
    public float cardSeperationDistance => _cardSeperationDistance;

    [Space(20)] 
    [SerializeField] private Vector2 _cardLaunchPosition;
    
    [Space(10)] 
    [SerializeField] private CardLauncher _cardLauncher;
    public CardLauncher cardLauncher => _cardLauncher;
    
    [SerializeField] private Transform _allCards;
    public Transform allCards => _allCards;

    [Space(20)] 
    [SerializeField][Range(0, 100)] private int _startingCardAmount;
    
    [SerializeField] private Card_ScrObj[] _startingCards;
    
    
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


    // Grid
    public bool Is_OuterGrid(Vector2 checkPosition)
    {
        if (checkPosition.x < _xGridRange.x || checkPosition.x > _xGridRange.y) return true;
        if (checkPosition.y < _yGridRange.x || checkPosition.y > _yGridRange.y) return true;
        
        return false;
    }

    public Vector2 Grid_ClampPosition(Vector2 position)
    {
        float xPos = Mathf.Clamp(position.x, _xGridRange.x, _xGridRange.y);
        float yPos = Mathf.Clamp(position.y, _yGridRange.x, _yGridRange.y);
        
        return new Vector2(xPos, yPos);
    }
    
    
    // New Cards
    private void Launch_StaticCard()
    {
        float randXPos = UnityEngine.Random.Range(_xGridRange.x, _xGridRange.y);
        float randYPos = UnityEngine.Random.Range(_yGridRange.x, _yGridRange.y);
        
        Card launchedCard = _cardLauncher.Launch_Card(_cardLaunchPosition, new(randXPos, randYPos));
        launchedCard.transform.SetParent(_allCards);
        
        int randIndex = UnityEngine.Random.Range(0, _startingCards.Length);
        
        launchedCard.Set_Data(new(_startingCards[randIndex]));
        launchedCard.Update_Visuals();
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

    public void UpdateCards_LayerOrder()
    {
        for (int i = 0; i < _currentCards.Count; i++)
        {
            Card card = _currentCards[i];

            if (card.detection.detectedCards.Count > 0) continue;
            card.sortingGroup.sortingOrder = 0;
        }
    }
}
