using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class Card : MonoBehaviour
{
    [Space(20)] 
    [SerializeField] private SpriteRenderer _cardBase;
    public SpriteRenderer cardBase => _cardBase;
    
    [SerializeField] private SpriteRenderer _icon;

    [Space(20)]
    [SerializeField] private GameObject _stackCardPrefab;
    [SerializeField] private Transform _allStackCards; // move to tableTop for movement effects
    [SerializeField] private Vector2 _stackOffset;

    [Space(20)] 
    [SerializeField] private SortingGroup _sortingGroup;
    public SortingGroup sortingGroup => _sortingGroup;
    
    [SerializeField] private IPointer_EventSystem _eventSystem;
    public IPointer_EventSystem eventSystem => _eventSystem;
    
    [SerializeField] private CardLauncher _cardLauncher;
    public CardLauncher cardLauncher => _cardLauncher;
    
    [Space(20)] 
    [SerializeField] private Card_Detection _detection;
    public Card_Detection detection => _detection;
    
    [SerializeField] private Card_Movement _movement;
    public Card_Movement movement => _movement;
    
    [SerializeField] private Card_Interaction _interaction;
    public Card_Interaction interaction => _interaction;

    [Space(20)]
    [SerializeField] private Vector2 _descriptionToggleOffset;


    private Card_Data _data;
    public Card_Data data => _data;

    
    // MonoBehaviour
    private void Awake()
    {
        _data = new(null);
    }

    private void Start()
    {
        Input_Controller input = Input_Controller.instance;
        Game_Controller controller = Game_Controller.instance;

        TableTop tableTop = controller.tableTop;
        Cursor cursor = controller.cursor;

        tableTop.Track_CurrentCard(this);
        
        // subscriptions
        _eventSystem.OnSelect += _movement.Toggle_DragDrop;
        _eventSystem.OnSelect += cursor.DragUpdate_CurrentCard;

        _eventSystem.OnSelect += cursor.Toggle_DragCardCount;

        // drag subscriptions
        _eventSystem.OnSelect += _movement.Dragging_Update;
        _eventSystem.OnMultiSelect += _movement.Dragging_Update;

        // pointer
        _movement.WhileDragging += _interaction.Point_ClosestCard;
        _eventSystem.OnSelect += _interaction.Interact_PointedCard;

        _eventSystem.OnSelect += _interaction.UpdateCards_Pointer;
        _eventSystem.OnMultiSelect += _interaction.UpdateCards_Pointer;
        _detection.OnCardExit += _interaction.UpdateCards_Pointer;

        // movement
        _eventSystem.OnSelect += _movement.Push_OverlappedCards;
        _eventSystem.OnSelect += _movement.UpdatePosition_OnInteract;

        // visual
        _eventSystem.OnSelect += tableTop.UpdateCards_LayerOrder;

        _eventSystem.OnSelect += Update_StackCards;
        _eventSystem.OnSelect += _movement.Update_Shadows;

        _eventSystem.OnSelect += cursor.UnToggle_CardDescriptions;
        _eventSystem.OnEnterDelay += cursor.Update_CardDescriptions;
        _eventSystem.OnExit += cursor.Update_CardDescriptions;
    }
    
    
    // Data
    public Card_Data Set_Data(Card_Data setData)
    {
        if (setData == null) return _data;
        
        _data = setData;
        return _data;
    }
    
    public Vector2 RandomPeripheral_SpawnPosition()
    {
        TableTop tableTop = Game_Controller.instance.tableTop;
        float launchRange = tableTop.cardSeperationDistance;
        
        float horizontalRange = Random.Range(-launchRange, launchRange);
        float verticalRange = Random.Range(-launchRange, launchRange);
        
        return (Vector2)transform.position + new Vector2(horizontalRange, verticalRange);
    }
    
    
    // Visual    
    public void Update_Visuals()
    {
        // _base.sprite = 
        _icon.sprite = _data.cardScrObj.iconSprite;
    }


    // Stack Card
    public Vector2 StackOffset(int stackCount)
    {
        if (stackCount <= 1) return Vector2.zero;
        return _stackOffset * (stackCount - 1);
    }
    public Vector2 Current_StackOffset()
    {
        int dragCount = Game_Controller.instance.cursor.currentCardDatas.Count;
        return StackOffset(dragCount);
    }

    public void Update_StackCards()
    {
        foreach (Transform stackCard in _allStackCards)
        {
            Destroy(stackCard.gameObject);
        }

        Game_Controller controller = Game_Controller.instance;
        
        Card currentDragCard = controller.tableTop.Current_DraggingCard();
        if (currentDragCard == null || currentDragCard != this) return;

        int stackAmount = controller.cursor.currentCardDatas.Count - 1;
        if (stackAmount <= 0) return;

        for (int i = 0; i < stackAmount; i++)
        {
            Vector2 spawnPos = _stackOffset * (i + 1) + (Vector2)transform.position;

            GameObject stackCardPrefab = Instantiate(_stackCardPrefab, spawnPos, Quaternion.identity);
            Transform stackCardTransform = stackCardPrefab.transform;

            stackCardTransform.SetParent(_allStackCards);
            stackCardTransform.SetAsFirstSibling();
        }
    }
}
