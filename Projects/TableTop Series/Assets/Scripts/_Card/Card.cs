using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Card : MonoBehaviour
{
    [Space(20)] 
    [SerializeField] private SortingGroup _sortingGroup;
    public SortingGroup sortingGroup => _sortingGroup;
    
    [SerializeField] private IPointer_EventSystem _eventSystem;
    
    [Space(20)] 
    [SerializeField] private Card_Detection _detection;
    public Card_Detection detection => _detection;
    
    [SerializeField] private Card_Movement _movement;
    public Card_Movement movement => _movement;
    
    
    private Card_Data _data;
    public Card_Data data => _data;
    
    
    // MonoBehaviour
    private void Start()
    {
        Game_Controller.instance.tableTop.currentCards.Add(this);
        
        // subscriptions
        _eventSystem.OnClick += _movement.Toggle_DragDrop;
        
        _eventSystem.OnClick += _movement.Push_OverlappedCards;
        _detection.OnCardDetection += _movement.Update_PushedMovement;
        
        _eventSystem.OnClick += _movement.Update_Shadows;
        _eventSystem.OnClick += Update_LayerOrder;
    }

    private void OnDestroy()
    {
        // subscriptions
        _eventSystem.OnClick -= _movement.Toggle_DragDrop;
        
        _eventSystem.OnClick -= _movement.Push_OverlappedCards;
        _detection.OnCardDetection -= _movement.Update_PushedMovement;

        _eventSystem.OnClick -= _movement.Update_Shadows;
        _eventSystem.OnClick -= Update_LayerOrder;
    }
    
    
    // Visual
    private void Update_LayerOrder()
    {
        if (_movement.dragging == false) return;
        _sortingGroup.sortingOrder = Game_Controller.instance.tableTop.Max_CardLayerOrder() + 1;
    }
}
