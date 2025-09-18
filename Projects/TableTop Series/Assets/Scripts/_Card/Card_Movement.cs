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


    private Camera _camera;
    
    private bool _dragging;
    public bool dragging => _dragging;

    private Vector2 _targetPosition;
    

    // MonoBehaviour
    private void Start()
    {
        _camera = Game_Controller.instance.mainCamera;

        _cardShadow.transform.localPosition = Vector2.zero;
    }

    private void Update()
    {
        TargetPosition_MovementUpdate();
        MouseFollow_Update();
    }


    // Drag and Drop
    public void Toggle_DragDrop()
    {
        Cursor cursor = Game_Controller.instance.cursor;
        if (cursor.currentCard != null && cursor.currentCard != _card) return;
        
        _dragging = !_dragging;
        Assign_TargetPosition(transform.position);
        
        Card setCard = _dragging ? _card : null;
        cursor.Set_CurrentCard(setCard);
    }
    
    private void MouseFollow_Update()
    {
        if (_dragging == false) return;
        
        Vector2 mousePos = Input.mousePosition;
        Vector2 targetPos = _camera.ScreenToWorldPoint(mousePos);
        
        transform.position = Vector2.Lerp(transform.position, targetPos, Time.deltaTime * _moveSpeed);
    }

    
    // Movements
    private bool At_TargetPosition()
    {
        float threshold = 0.1f;
        float distanceFromTarget = Vector2.Distance(transform.position, _targetPosition);
        
        return distanceFromTarget < threshold;
    }

    private void TargetPosition_MovementUpdate()
    {
        if (_dragging) return;
        if (At_TargetPosition()) return;
        
        transform.position = Vector2.Lerp(transform.position, _targetPosition, _moveSpeed * 0.1f * Time.deltaTime);
    }
    
    public void Assign_TargetPosition(Vector2 assignPosition)
    {
        _targetPosition = assignPosition;
    }
    
    
    // Effects
    public void Update_Shadows()
    {
        Vector2 updatePos = _dragging ? _shadowOffset : Vector2.zero;
        
        LeanTween.cancel(_cardShadow);
        LeanTween.moveLocal(_cardShadow, updatePos, _shadowSpeed);
    }
}