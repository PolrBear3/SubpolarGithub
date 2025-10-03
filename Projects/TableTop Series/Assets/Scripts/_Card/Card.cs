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
    [SerializeField] private SortingGroup _sortingGroup;
    public SortingGroup sortingGroup => _sortingGroup;
    
    [SerializeField] private IPointer_EventSystem _eventSystem;
    
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
        
        TableTop tableTop = Game_Controller.instance.tableTop;
        tableTop.currentCards.Add(this);
        
        // subscriptions
        _eventSystem.OnSelect += _movement.Toggle_DragDrop;

        // drag subscriptions
        _eventSystem.OnSelect += _movement.Dragging_Update;
        _eventSystem.OnMultiSelect += _movement.Dragging_Update;

        // pointer
        _eventSystem.OnIdle += Toggle_Description;
        _eventSystem.OnPoint += Toggle_Description;
        
        _movement.WhileDragging += _interaction.Point_ClosestCard;
        _eventSystem.OnSelect += _interaction.Interact_PointedCard;

        _eventSystem.OnSelect += _interaction.UpdateCards_Pointer;
        _eventSystem.OnMultiSelect += _interaction.UpdateCards_Pointer;
        _detection.OnCardExit += _interaction.UpdateCards_Pointer;

        // movement
        _eventSystem.OnSelect += _movement.Push_OverlappedCards;
        _eventSystem.OnMultiSelect += _movement.Push_OverlappedCards;
        
        _detection.OnCardDetection += _movement.Update_PushedMovement;
        tableTop.OnLoopUpdate += _movement.Update_PushedMovement;
        
        tableTop.OnLoopUpdate += _movement.Update_OuterPosition;

        // visual
        _eventSystem.OnSelect += tableTop.UpdateCards_LayerOrder;
        _eventSystem.OnSelect += Update_LayerOrder;

        _eventSystem.OnMultiSelect += tableTop.UpdateCards_LayerOrder;
        _eventSystem.OnMultiSelect += Update_LayerOrder;

        _eventSystem.OnSelect += _movement.Update_Shadows;
        _eventSystem.OnMultiSelect += _movement.Update_Shadows;
    }

    private void OnDestroy()
    {
        Input_Controller input = Input_Controller.instance;
        TableTop tableTop = Game_Controller.instance.tableTop;
        
        // subscriptions
        _eventSystem.OnSelect -= _movement.Toggle_DragDrop;

        // drag subscriptions
        _eventSystem.OnSelect -= _movement.Dragging_Update;
        _eventSystem.OnMultiSelect -= _movement.Dragging_Update;

        // pointer
        _eventSystem.OnIdle -= Toggle_Description;
        _eventSystem.OnPoint -= Toggle_Description;
        
        _movement.WhileDragging -= _interaction.Point_ClosestCard;
        _eventSystem.OnSelect -= _interaction.Interact_PointedCard;

        _eventSystem.OnSelect -= _interaction.UpdateCards_Pointer;
        _eventSystem.OnMultiSelect -= _interaction.UpdateCards_Pointer;
        _detection.OnCardExit -= _interaction.UpdateCards_Pointer;

        // movement
        _eventSystem.OnSelect -= _movement.Push_OverlappedCards;
        _eventSystem.OnMultiSelect -= _movement.Push_OverlappedCards;

        _detection.OnCardDetection -= _movement.Update_PushedMovement;
        tableTop.OnLoopUpdate -= _movement.Update_PushedMovement;

        tableTop.OnLoopUpdate -= _movement.Update_OuterPosition;

        // visual
        _eventSystem.OnSelect -= tableTop.UpdateCards_LayerOrder;
        _eventSystem.OnSelect -= Update_LayerOrder;

        _eventSystem.OnMultiSelect -= tableTop.UpdateCards_LayerOrder;
        _eventSystem.OnMultiSelect -= Update_LayerOrder;

        _eventSystem.OnSelect -= _movement.Update_Shadows;
        _eventSystem.OnMultiSelect -= _movement.Update_Shadows;
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
    private void Update_LayerOrder()
    {
        if (_movement.dragging == false) return;
        _sortingGroup.sortingOrder = Game_Controller.instance.tableTop.Max_CardLayerOrder() + 1;
    }
    
    private void Toggle_Description()
    {
        Input_Controller input = Input_Controller.instance;
        Cursor cursor = Game_Controller.instance.cursor;

        bool toggle = input.isIdle;
        Card infoCard = toggle ? this : null;
        
        cursor.Update_HoverCardInfo(infoCard);

        if (!toggle) return;
        cursor.Update_CursorPoint((Vector2)transform.position + _descriptionToggleOffset);
    }
    
    
    public void Update_Visuals()
    {
        // _base.sprite = 
        _icon.sprite = _data.cardScrObj.iconSprite;
    }
}
