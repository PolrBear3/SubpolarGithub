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
    [SerializeField][Range(0, 10)] private float _pushDelayTime;

    [Space(20)]
    [SerializeField] private Vector2 _defaultOffset;
    [SerializeField] private Vector2 _shadowOffset;
    [SerializeField][Range(0, 10)] private float _shadowSpeed;
    
    [Space(20)] 
    [SerializeField][Range(0, 100)] private float _moveSpeed;
    [SerializeField][Range(0, 10)] private float _moveBreakValue;


    private bool _dragging;
    public bool dragging => _dragging;

    public Action WhileDragging;

    private Vector2 _targetPosition;
    public Vector2 targetPosition => _targetPosition;

    private Vector2 _currentVelocity;

    private Coroutine _draggingUpdateCoroutine;
    private Coroutine _pushUpdateCoroutine;
    

    // MonoBehaviour
    private void Start()
    {
        _camera = Game_Controller.instance.mainCamera;

        Update_Shadows();
    }
    
    private void Update()
    {
        TargetPosition_MovementUpdate();
        DragPosition_Update();
    }


    // Drag and Drop
    private Vector2 SnapPosition(Vector2 position)
    {
        TableTop tableTop = Game_Controller.instance.tableTop;
        List<Vector2> dropPositions = tableTop.CardSnapPoints(position);

        if (dropPositions.Count == 0) return transform.position;
        return dropPositions[0];
    }

    public void Toggle_DragDrop(bool toggle)
    {
        Game_Controller controller = Game_Controller.instance;

        Cursor cursor = controller.cursor;
        List<Card_Data> dragDatas = cursor.currentCardDatas;

        _dragging = toggle;

        if (_dragging && dragDatas.Contains(_card.data)) return;

        if (_dragging)
        {
            dragDatas.Add(_card.data);
            return;
        }
        dragDatas.Remove(_card.data);

        Vector2 snapPos = SnapPosition(transform.position);
        Assign_TargetPosition(snapPos);
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


    // Card to Card Push Update
    private List<Card> DistanceOverlapped_Cards()
    {
        TableTop tableTop = Game_Controller.instance.tableTop;

        List<Card> allCards = tableTop.currentCards;
        float seperationDistance = tableTop.cardSeperationDistance;

        List<Card> overlappedCards = new();

        for (int i = 0; i < allCards.Count; i++)
        {
            if (allCards[i] == _card) continue;

            Vector2 cardPos = allCards[i].movement.targetPosition;
            float distance = (_targetPosition - cardPos).sqrMagnitude;

            if (distance > seperationDistance) continue;
            overlappedCards.Add(allCards[i]);
        }

        return overlappedCards;
    }

    public void Push_OverlappedCards()
    {
        if (_pushUpdateCoroutine != null)
        {
            StopCoroutine(_pushUpdateCoroutine);
            _pushUpdateCoroutine = null;
        }

        if (_dragging) return;
        if (_card.interaction.interactedCard != null) return;

        _pushUpdateCoroutine = StartCoroutine(OverlappedCards_PushCoroutine());
    }
    private IEnumerator OverlappedCards_PushCoroutine()
    {
        yield return new WaitForSeconds(_pushDelayTime);

        List<Card> overlappedCards = DistanceOverlapped_Cards();

        if (overlappedCards.Count == 0)
        {
            _pushUpdateCoroutine = null;
            yield break;
        }

        TableTop tableTop = Game_Controller.instance.tableTop;
        List<Vector2> pushPositions = new(tableTop.Surrounding_CardSnapPoints(_targetPosition));

        for (int i = 0; i < overlappedCards.Count; i++)
        {
            Card_Movement overlapCardMovement = overlappedCards[i].movement;

            // calculates direction to exact drop position on same position overlap
            bool targetPositionMatch = overlapCardMovement.targetPosition == _targetPosition;
            Vector2 dropPos = targetPositionMatch ? transform.position : _targetPosition;

            // for diagonal snappoints
            Vector2 direction = (overlapCardMovement.targetPosition - dropPos).normalized;
            Vector2 pushPos = direction * tableTop.cardSeperationDistance + _targetPosition;

            int closestPosIndex = 0;

            for (int j = 0; j < pushPositions.Count; j++)
            {
                if (j == closestPosIndex) continue;

                float closestDistance = (pushPos - pushPositions[closestPosIndex]).sqrMagnitude;
                float checkDistance = (pushPos - pushPositions[j]).sqrMagnitude;

                if (checkDistance >= closestDistance) continue;
                closestPosIndex = j;
            }

            // outer grid position restriction
            Vector2 snapPushPos = pushPositions[closestPosIndex];
            snapPushPos = tableTop.Is_OuterGrid(snapPushPos) ? SnapPosition(snapPushPos) : snapPushPos;

            overlapCardMovement.Assign_TargetPosition(snapPushPos);
            overlapCardMovement.Push_OverlappedCards();

            pushPositions.RemoveAt(closestPosIndex);
        }

        _pushUpdateCoroutine = null;
    }

    public void UpdatePosition_OnInteract()
    {
        if (_dragging) return;

        Card_Interaction interaction = _card.interaction;
        Card interactedCard = interaction.interactedCard;

        if (interactedCard == null) return;

        TableTop tableTop = Game_Controller.instance.tableTop;

        Vector2 interactCardPos = interactedCard.movement.targetPosition;
        Vector2 droppedPos = transform.position;

        Vector2 pushDirection = (droppedPos - interactCardPos).normalized;
        Vector2 pushPos = pushDirection * tableTop.cardSeperationDistance + droppedPos;

        List<Vector2> pushedPositions = new(tableTop.Surrounding_CardSnapPoints(interactCardPos));
        int closestPosIndex = 0;

        for (int i = 0; i < pushedPositions.Count; i++)
        {
            if (i == closestPosIndex) continue;

            float closestDistance = (pushPos - pushedPositions[closestPosIndex]).sqrMagnitude;
            float checkDistance = (pushPos - pushedPositions[i]).sqrMagnitude;

            if (checkDistance >= closestDistance) continue;
            closestPosIndex = i;
        }

        interaction.Reset_InteractData();

        Assign_TargetPosition(pushedPositions[closestPosIndex]);
        Push_OverlappedCards();
    }


    // Visual
    public void Update_Shadows()
    {
        Vector2 updatePos = _dragging ? _shadowOffset : _defaultOffset;
        
        LeanTween.cancel(_cardShadow);
        LeanTween.moveLocal(_cardShadow, updatePos, _shadowSpeed);
    }
}