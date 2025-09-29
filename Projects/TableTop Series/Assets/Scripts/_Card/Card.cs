using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class Card : MonoBehaviour
{
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
    
    
    private Card_Data _data;
    public Card_Data data => _data;
    
    
    // MonoBehaviour
    private void Awake()
    {
        _data = new(null);
    }

    private void Start()
    {
        TableTop tableTop = Game_Controller.instance.tableTop;
        tableTop.currentCards.Add(this);
        
        // subscriptions
        _eventSystem.OnClick += _movement.Toggle_DragDrop;
        
        _eventSystem.OnClick += _movement.Dragging_Update;
        _movement.WhileDragging += _interaction.Point_ClosestCard;

        // _eventSystem.OnClick += _interaction.Spawn_StackedCard_OnEmpty;
        _eventSystem.OnClick += _interaction.Interact_PointedCard;

        _eventSystem.OnClick += _interaction.UpdateCards_Pointer;
        _detection.OnCardExit += _interaction.UpdateCards_Pointer;

        _eventSystem.OnClick += _movement.Push_OverlappedCards;
        _detection.OnCardDetection += _movement.Update_PushedMovement;
        
        _eventSystem.OnClick += tableTop.UpdateCards_LayerOrder;
        _eventSystem.OnClick += Update_LayerOrder;
        
        _eventSystem.OnClick += _movement.Update_Shadows;
    }

    private void OnDestroy()
    {
        // subscriptions
        _eventSystem.OnClick -= _movement.Toggle_DragDrop;
        
        _eventSystem.OnClick -= _movement.Dragging_Update;
        _movement.WhileDragging -= _interaction.Point_ClosestCard;
        
        // _eventSystem.OnClick -= _interaction.Spawn_StackedCard_OnEmpty;
        _eventSystem.OnClick -= _interaction.Interact_PointedCard;
        
        _eventSystem.OnClick -= _interaction.UpdateCards_Pointer;
        _detection.OnCardExit -= _interaction.UpdateCards_Pointer;

        _eventSystem.OnClick -= _movement.Push_OverlappedCards;
        _detection.OnCardDetection -= _movement.Update_PushedMovement;
        
        _eventSystem.OnClick -= Game_Controller.instance.tableTop.UpdateCards_LayerOrder;
        _eventSystem.OnClick -= Update_LayerOrder;
        
        _eventSystem.OnClick -= _movement.Update_Shadows;
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
}
