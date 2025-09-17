using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Movement : MonoBehaviour
{
    [Space(20)] 
    [SerializeField] private Card _card;
    [SerializeField] private GameObject _cardShadow;
    
    [Space(20)] 
    [SerializeField] private Vector2 _shadowOffset;
    
    [Space(10)] 
    [SerializeField][Range(0, 100)] private float _moveSpeed;
    [SerializeField][Range(0, 10)] private float _shadowSpeed;

    
    private bool _dragging;
    public bool dragging => _dragging;
    

    // MonoBehaviour
    private void Start()
    {
        _cardShadow.transform.localPosition = Vector2.zero;
    }

    private void Update()
    {
        MouseFollow_Update();
    }


    // Main
    public void Toggle_DragDrop()
    {
        Cursor cursor = Game_Controller.instance.cursor;
        if (cursor.currentCard != null && cursor.currentCard != _card) return;
        
        _dragging = !_dragging;
        
        Card setCard = _dragging ? _card : null;
        cursor.Set_CurrentCard(setCard);
    }
    
    private void MouseFollow_Update()
    {
        if (_dragging == false) return;
        
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.WorldToScreenPoint(transform.position).z; 
        
        Vector3 targetPos = Camera.main.ScreenToWorldPoint(mousePos);
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * _moveSpeed);
    }

    
    // Effects
    public void Update_Shadows()
    {
        Vector2 updatePos = _dragging ? _shadowOffset : Vector2.zero;
        
        LeanTween.cancel(_cardShadow);
        LeanTween.moveLocal(_cardShadow, updatePos, _shadowSpeed);
    }
}