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

        cursor.Update_HoverCardInfo(setCard); // test
        
        if (_dragging) return;
        
        Assign_TargetPosition(transform.position);
        Update_OuterPosition();
    }
    
    private void MouseFollow_Update()
    {
        if (_dragging == false) return;
        
        Vector2 mousePos = Input.mousePosition;
        Vector2 targetPos = _camera.ScreenToWorldPoint(mousePos);
        
        transform.position = Vector2.Lerp(transform.position, targetPos, Time.deltaTime * _moveSpeed);
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

        _outerPositionCoroutine = StartCoroutine(OuterPosition_UpdateCoroutine());
    }
    private IEnumerator OuterPosition_UpdateCoroutine()
    {
        TableTop tableTop = Game_Controller.instance.tableTop;

        while (Is_Moving()) yield return null;
        
        if (tableTop.Is_OuterGrid(transform.position) == false) yield break;
        Assign_TargetPosition(tableTop.Grid_ClampPosition(transform.position));

        _outerPositionCoroutine = null;
    }
    
    
    // Seperation
    private Vector2 Pushed_TargetPosition(Vector2 pushStartPos, Vector2 pushedCardPosition)
    {
        TableTop tableTop = Game_Controller.instance.tableTop;
        
        float seperationDistance = tableTop.cardSeperationDistance;
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
            if (detectedCards[i] == null) continue;
            Card_Movement movement = detectedCards[i].movement;
            
            Vector2 detectedCardPos = detectedCards[i].transform.position;
            Vector2 pushedPos = Pushed_TargetPosition(detectedCardPos);
            
            movement.Assign_TargetPosition(pushedPos);
            movement.Update_OuterPosition();
        }
    }
    
    public void Update_PushedMovement()
    {
        if (_dragging) return;
        if (Is_Moving()) return;

        List<Card> detectedCards = _card.detection.detectedCards;
        
        for (int i = 0; i < detectedCards.Count; i++)
        {
            if (detectedCards[i] == null) continue;
            
            Card_Movement pushingCardMovement = detectedCards[i].movement;
            if (pushingCardMovement.dragging) continue;

            Vector2 pushingCardTargetPos = pushingCardMovement._targetPosition;
        
            float pushDistance = Game_Controller.instance.tableTop.cardSeperationDistance;
            Vector2 pushedPosition = Pushed_TargetPosition(pushingCardTargetPos, transform.position);
        
            Assign_TargetPosition(pushedPosition);
            Update_OuterPosition();

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