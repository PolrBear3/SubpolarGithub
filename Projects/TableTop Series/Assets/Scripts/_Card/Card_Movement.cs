using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Card_Movement : MonoBehaviour
{
    private Camera _camera;
    
    
    [Space(20)]
    [SerializeField] private Card _card;
    [SerializeField] private GameObject _cardShadow;

    [Space(20)] 
    [SerializeField][Range(0, 10)] private float _dragTikTime;
    
    [Space(20)]
    [SerializeField] private Vector2 _shadowOffset;
    [SerializeField][Range(0, 10)] private float _shadowSpeed;
    
    [Space(20)] 
    [SerializeField][Range(0, 100)] private float _moveSpeed;
    [SerializeField][Range(0, 10)] private float _moveBreakValue;


    private bool _dragging;
    public bool dragging => _dragging;

    public Action WhileDragging;

    private Vector2 _targetPosition;
    private Vector2 _currentVelocity;

    private Coroutine _draggingUpdateCoroutine;
    private Coroutine _outerPositionCoroutine;
    

    // MonoBehaviour
    private void Start()
    {
        _camera = Game_Controller.instance.mainCamera;

        _cardShadow.transform.localPosition = Vector2.zero;
    }
    
    private void Update()
    {
        TargetPosition_MovementUpdate();
        DragPosition_Update();
    }


    // Drag and Drop
    public void Toggle_DragDrop(bool toggle)
    {
        Cursor cursor = Game_Controller.instance.cursor;
        List<Card_Data> dragDatas = cursor.currentCardDatas;
        
        _dragging = toggle;

        if (_dragging && dragDatas.Contains(_card.data)) return;

        if (_dragging)
        {
            dragDatas.Add(_card.data);
            return;
        }
        dragDatas.Remove(_card.data);

        cursor.Update_CursorPoint();
        Vector2 dropPos = cursor.Card_DropPoint();

        transform.position = dropPos;
        Assign_TargetPosition(dropPos);

        Update_OuterPosition();
    }
    public void Toggle_DragDrop()
    {
        Card currentDragCard = Game_Controller.instance.tableTop.Current_DraggingCard();
        if (currentDragCard != null && currentDragCard != _card) return;

        Toggle_DragDrop(!_dragging);
    }

    private void DragPosition_Update()
    {
        if (_dragging == false) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 dragPos = _camera.ScreenToWorldPoint(mousePos);

        transform.position = Vector2.Lerp(transform.position, dragPos, Time.deltaTime * _moveSpeed);
    }

    public void Dragging_Update()
    {
        if (_draggingUpdateCoroutine != null)
        {
            StopCoroutine(_draggingUpdateCoroutine);
            _draggingUpdateCoroutine = null;
        }

        if (_dragging == false) return;
        _draggingUpdateCoroutine = StartCoroutine(DraggingUpdate_Coroutine());
    }
    private IEnumerator DraggingUpdate_Coroutine()
    {
        while (_dragging)
        {
            WhileDragging?.Invoke();
            yield return new WaitForSeconds(_dragTikTime);
        }
        _draggingUpdateCoroutine = null;
    }

    
    // Target Position Movement
    public bool Is_Moving()
    {
        return Vector2.Distance(transform.position, _targetPosition) > 0.01f;
    }

    public void Assign_TargetPosition(Vector2 targetPosition)
    {
        _targetPosition = targetPosition;
        _card.data.Update_DroppedPosition(_targetPosition);
    }

    private void TargetPosition_MovementUpdate()
    {
        if (_dragging) return;
        if (Is_Moving() == false) return;

        float breakValue = _moveBreakValue * 0.1f;
        transform.position = Vector2.SmoothDamp(transform.position, _targetPosition, ref _currentVelocity, breakValue, _moveSpeed);
    }

    public void Update_OuterPosition()
    {
        if (_outerPositionCoroutine != null)
        {
            StopCoroutine(_outerPositionCoroutine);
            _outerPositionCoroutine = null;
        }

        if (_dragging) return;
        if (Is_Moving()) return;

        _outerPositionCoroutine = StartCoroutine(OuterPosition_UpdateCoroutine());
    }
    private IEnumerator OuterPosition_UpdateCoroutine()
    {
        TableTop tableTop = Game_Controller.instance.tableTop;

        while (Is_Moving()) yield return null;
        
        if (tableTop.Is_OuterGrid(transform.position) == false) yield break;
        Assign_TargetPosition(tableTop.InnerGrid_Position(transform.position));

        _outerPositionCoroutine = null;
    }
    
    
    // Seperation
    private Vector2 Pushed_TargetPosition(Vector2 pushingCardPos, Vector2 pushedCardPos)
    {
        float seperationDistance = Game_Controller.instance.tableTop.cardSeperationDistance;
        float currentDistance = Vector2.Distance(pushingCardPos, pushedCardPos);
        
        if (currentDistance >= seperationDistance) return pushedCardPos;

        Vector2 pushDirection = pushedCardPos - pushingCardPos;
        float pushDistance = seperationDistance - currentDistance;
        
        float diagonalFactor = Mathf.Abs(pushDirection.normalized.x) + Mathf.Abs(pushDirection.normalized.y);
        diagonalFactor = Mathf.Clamp(diagonalFactor, 1f, Mathf.Sqrt(2f));
        
        pushDistance *= diagonalFactor;
        
        return pushedCardPos + pushDirection.normalized * pushDistance;
    }
    
    public void Push_OverlappedCards()
    {
        if (_dragging) return;
        if (_card.interaction.interacted) return;

        List<Card> detectedCards = _card.detection.detectedCards;
        if (detectedCards.Count == 0) return;

        for (int i = 0; i < detectedCards.Count; i++)
        {
            if (detectedCards[i] == null) continue;
            Card_Movement movement = detectedCards[i].movement;
            
            Vector2 detectedCardPos = detectedCards[i].transform.position;
            Vector2 pushedPos = Pushed_TargetPosition(transform.position, detectedCardPos);
            
            movement.Assign_TargetPosition(pushedPos);
        }
    }
    
    public void Update_PushedMovement()
    {
        if (_dragging) return;
        if (Is_Moving()) return;

        List<Card> detectedCards = _card.detection.detectedCards;
        if (detectedCards.Count == 0) return;

        for (int i = 0; i < detectedCards.Count; i++)
        {
            if (detectedCards[i] == null) continue;
            if (detectedCards[i].interaction.interacted) continue;
            
            Card_Movement pushingCardMovement = detectedCards[i].movement;
            if (pushingCardMovement.dragging) continue;

            Vector2 pushingCardTargetPos = pushingCardMovement._targetPosition;
            Vector2 pushedPosition = Pushed_TargetPosition(pushingCardTargetPos, transform.position);
        
            Assign_TargetPosition(pushedPosition);
            return;
        }
    }
    
    
    // Visual
    public void Update_Shadows()
    {
        Vector2 updatePos = _dragging ? _shadowOffset : Vector2.zero;
        
        LeanTween.cancel(_cardShadow);
        LeanTween.moveLocal(_cardShadow, updatePos, _shadowSpeed);
    }
}