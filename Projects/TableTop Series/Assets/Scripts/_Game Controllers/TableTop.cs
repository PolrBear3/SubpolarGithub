using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableTop : MonoBehaviour
{
    [Space(20)] 
    [SerializeField][Range(0, 10)] private float _loopUpdateTikTime;
    
    [Space(10)] 
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


    private List<Vector2> _cardSnapPoints = new();

    private List<Card> _currentCards = new();
    public List<Card> currentCards => _currentCards;

    private Coroutine _loopUpdateCoroutine;
    public Action OnLoopUpdate;


    // MonoBehaviour
    private void Awake()
    {
        Load_CardSnapPoints();
    }

    private void Start()
    {
        Toggle_LoopUpdate(true);
        StartCoroutine(LaunchCards_Coroutine());
    }


    // Main
    private void Toggle_LoopUpdate(bool toggle)
    {
        if (_loopUpdateCoroutine != null)
        {
            StopCoroutine(_loopUpdateCoroutine);
            _loopUpdateCoroutine = null;
        }
        
        if (toggle == false) return;

        _loopUpdateCoroutine = StartCoroutine(LoopUpdate_Coroutine());
    }
    private IEnumerator LoopUpdate_Coroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_loopUpdateTikTime);
            OnLoopUpdate?.Invoke();
        }
    }
    
    
    // Grid
    private void Load_CardSnapPoints()
    {
        float snapPointDistance = _cardSeperationDistance / 2;

        float minX = Mathf.Ceil(_xGridRange.x / snapPointDistance) * snapPointDistance;
        float maxX = Mathf.Floor(_xGridRange.y / snapPointDistance) * snapPointDistance;
        float minY = Mathf.Ceil(_yGridRange.x / snapPointDistance) * snapPointDistance;
        float maxY = Mathf.Floor(_yGridRange.y / snapPointDistance) * snapPointDistance;

        for (float y = maxY; y >= minY; y -= snapPointDistance)
        {
            for (float x = minX; x <= maxX; x += snapPointDistance)
            {
                _cardSnapPoints.Add(new Vector2(x, y));
            }
        }
    }

    public List<Vector2> CardSnapPoints(Vector2 position)
    {
        List<Vector2> snapPoints = new(_cardSnapPoints);

        snapPoints.Sort((a, b) =>
        {
            float distA = Vector2.Distance(a, position);
            float distB = Vector2.Distance(b, position);
            return distA.CompareTo(distB);
        });

        return snapPoints;
    }

    public bool Is_OuterGrid(Vector2 checkPosition)
    {
        if (checkPosition.x < _xGridRange.x || checkPosition.x > _xGridRange.y) return true;
        if (checkPosition.y < _yGridRange.x || checkPosition.y > _yGridRange.y) return true;

        return false;
    }


    // Current Cards
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

    public Card Current_DraggingCard()
    {
        for (int i = 0; i < _currentCards.Count; i++)
        {
            if (!_currentCards[i].movement.dragging) continue;
            return _currentCards[i];
        }
        return null;
    }

    public bool Card_OverlapPosition(Vector2 checkPosition)
    {
        for (int i = 0; i < _currentCards.Count; i++)
        {
            Card_Movement movement = _currentCards[i].movement;
            if (movement.dragging) continue;

            float distance = Vector2.Distance(checkPosition, movement.targetPosition);
            if (distance > _cardSeperationDistance) continue;

            return true;
        }
        return false;
    }


    // Current Card Layers
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
