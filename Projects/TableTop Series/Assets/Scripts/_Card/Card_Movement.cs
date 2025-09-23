using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Movement : MonoBehaviour
{
    private Camera _camera;
    
    
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

    private Vector2 _targetPosition;
    

    // MonoBehaviour
    private void Start()
    {
        _camera = Game_Controller.instance.mainCamera;
        
        _targetPosition = transform.position;
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
        Game_Controller controller = Game_Controller.instance;
        
        Cursor cursor = controller.cursor;
        if (cursor.currentCard != null && cursor.currentCard != _card) return;
        
        _dragging = !_dragging;
        
        Card setCard = _dragging ? _card : null;
        cursor.Set_CurrentCard(setCard);

        if (_dragging) return;
        Assign_TargetPosition(transform.position);
    }
    
    private void MouseFollow_Update()
    {
        if (_dragging == false) return;
        
        Vector2 mousePos = Input.mousePosition;
        Vector2 targetPos = _camera.ScreenToWorldPoint(mousePos);
        
        transform.position = Vector2.Lerp(transform.position, targetPos, Time.deltaTime * _moveSpeed);
    }

    
    // Target Direction Movement
    public bool Is_Moving()
    {
        return Vector2.Distance(transform.position, _targetPosition) > 0.01f;
    }
    
    public void Assign_TargetPosition(Vector2 targetPosition)
    {
        _targetPosition = targetPosition;
    }
    
    private void TargetPosition_MovementUpdate()
    {
        if (_dragging) return;
        if (Is_Moving() == false) return;

        transform.position = Vector2.Lerp(transform.position, _targetPosition, Time.deltaTime * _moveSpeed);
    }
    
    
    // Seperation
    private Vector2 Pushed_TargetPosition(Vector2 pushStartPos, Vector2 pushedCardPosition)
    {
        float seperationDistance = Game_Controller.instance.tableTop.cardSeperationDistance;
        float currentDistance = Vector2.Distance(pushStartPos, pushedCardPosition);
        
        if (currentDistance >= seperationDistance) return pushedCardPosition;

        Vector2 pushDirection = pushedCardPosition - pushStartPos;
        float pushDistance = seperationDistance - currentDistance;
        
        return pushedCardPosition + pushDirection.normalized * pushDistance;
    }
    private Vector2 Pushed_TargetPosition(Vector2 pushedCardPosition)
    {
        return Pushed_TargetPosition(transform.position, pushedCardPosition);
    }
    
    public void Push_OverlappedCards()
    {
        if (_dragging) return;

        List<Card> detectedCards = _card.detection.detectedCards;

        for (int i = 0; i < detectedCards.Count; i++)
        {
            Card_Movement movement = detectedCards[i].movement;
            
            Vector2 detectedCardPos = detectedCards[i].transform.position;
            Vector2 pushedPos = Pushed_TargetPosition(detectedCardPos);
            
            movement.Assign_TargetPosition(pushedPos);
        }
    }
    
    public void Update_PushedMovement()
    {
        if (_dragging) return;
        if (Is_Moving()) return;

        Card_Movement pushingCardMovement = _card.detection.detectedCards[0].movement;
        if (pushingCardMovement._dragging) return;
        
        Vector2 pushingCardTargetPos = pushingCardMovement._targetPosition;
        
        float pushDistance = Game_Controller.instance.tableTop.cardSeperationDistance;
        Vector2 pushedPosition = Pushed_TargetPosition(pushingCardTargetPos, transform.position);
        
        Assign_TargetPosition(pushedPosition);
    }
    
    
    // Effects
    public void Update_Shadows()
    {
        Vector2 updatePos = _dragging ? _shadowOffset : Vector2.zero;
        
        LeanTween.cancel(_cardShadow);
        LeanTween.moveLocal(_cardShadow, updatePos, _shadowSpeed);
    }
}