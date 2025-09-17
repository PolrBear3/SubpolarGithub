using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    [Space(20)] 
    [SerializeField] private IPointer_EventSystem _eventSystem;
    
    [Space(20)] 
    [SerializeField] private Card_Movement _movement;
    public Card_Movement movement => _movement;
    
    
    private Card_Data _data;
    public Card_Data data => _data;
    
    
    // MonoBehaviour
    private void Start()
    {
        // subscriptions
        _eventSystem.OnClick += _movement.Toggle_DragDrop;
        _eventSystem.OnClick += _movement.Update_Shadows;
    }

    private void OnDestroy()
    {
        // subscriptions
        _eventSystem.OnClick -= _movement.Toggle_DragDrop;
        _eventSystem.OnClick -= _movement.Update_Shadows;
    }
}
