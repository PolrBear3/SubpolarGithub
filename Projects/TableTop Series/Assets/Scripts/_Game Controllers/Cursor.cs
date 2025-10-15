using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Cursor : MonoBehaviour
{
    private Camera _camera;

    
    [Space(20)]
    [SerializeField] private RectTransform _uiCursorPoint;
    [SerializeField] private GameObject _emptyCardPrefab;
    
    [Space(20)] 
    [SerializeField] private RectTransform _cardDescription;
    [SerializeField] private TextMeshProUGUI cardDescriptionText;

    [Space(20)] 
    [SerializeField] private RectTransform _dragCardCountBox;
    [SerializeField] private TextMeshProUGUI _dragCardCountText;

    [Space(20)]
    [SerializeField] private RectTransform _cardDropPoint;


    private List<Card_Data> _currentCardDatas = new();
    public List<Card_Data> currentCardDatas => _currentCardDatas;

    private Card _recentDragCard;


    private bool _cursorPointActive;


    // MonoBehaviour
    private void Start()
    {
        Game_Controller controller = Game_Controller.instance;

        _camera = controller.mainCamera;

        Toggle_DragCardCount();

        // subscriptions
        Input_Controller input = Input_Controller.instance;
        TableTop tableTop = controller.tableTop;

        input.OnMultiSelect += Drag_MultipleCards;
        input.OnMultiSelect += DropAll_CurrentCards;

        input.OnMultiSelect += tableTop.UpdateCards_LayerOrder;

        input.OnMultiSelect += Toggle_DragCardCount;
        input.OnPoint += Toggle_DragCardCount;
        input.OnIdle += Toggle_DragCardCount;
    }

    private void OnDestroy()
    {
        // subscriptions
        Input_Controller input = Input_Controller.instance;
        TableTop tableTop = Game_Controller.instance.tableTop;

        input.OnMultiSelect -= Drag_MultipleCards;
        input.OnMultiSelect -= DropAll_CurrentCards;

        input.OnMultiSelect -= tableTop.UpdateCards_LayerOrder;

        input.OnMultiSelect -= Toggle_DragCardCount;
        input.OnPoint -= Toggle_DragCardCount;
        input.OnIdle -= Toggle_DragCardCount;
    }

    private void Update()
    {
        CursorPoint_Update();
    }


    // Curor Point
    public void Toggle_CursorPointUpdate(bool toggle)
    {
        _cursorPointActive = toggle;

        if (!toggle) return;
        Update_CursorPoint();
    }
    private void CursorPoint_Update()
    {
        if (!_cursorPointActive) return;

        Update_CursorPoint();
    }

    public void Update_CursorPoint(Vector2 pointPosition)
    {
        _uiCursorPoint.position = _camera.WorldToScreenPoint(pointPosition);
    }
    public void Update_CursorPoint()
    {
        _uiCursorPoint.position = Mouse.current.position.ReadValue();
    }

    public Vector2 Card_DropPoint()
    {
        return _camera.ScreenToWorldPoint(_cardDropPoint.position);
    }


    // Current Cards Data Control
    private void Drag_MultipleCards()
    {
        if (_currentCardDatas.Count == 0) return;

        TableTop tableTop = Game_Controller.instance.tableTop;
        Card currentCard = tableTop.Current_DraggingCard();

        List<Card> overlappedCards = currentCard.detection.Closest_DetectedCards();

        if (overlappedCards.Count == 0)
        {
            _recentDragCard = null;
            return;
        }

        Card additionalTargetCard = overlappedCards[0];
        _recentDragCard = additionalTargetCard;

        Card_Data additionalCardData = additionalTargetCard.data;
        _currentCardDatas.Add(additionalCardData);

        tableTop.currentCards.Remove(additionalTargetCard);
        Destroy(additionalTargetCard.gameObject);

        currentCard.Set_Data(additionalCardData);

        currentCard.Update_Visuals();
        currentCard.Update_StackCards();
    }
    private void DropAll_CurrentCards()
    {
        if (_currentCardDatas.Count == 0) return;

        if (_recentDragCard != null) return;
        _recentDragCard = null;

        TableTop tableTop = Game_Controller.instance.tableTop;
        Card currentCard = tableTop.Current_DraggingCard();

        List<Card> overlappedCards = currentCard.detection.Closest_DetectedCards();
        if (overlappedCards.Count > 0) return;

        Vector2 cursorPos = currentCard.transform.position;

        List<Vector2> dropPositions = tableTop.CardSnapPoints(cursorPos);
        int dropCount = _currentCardDatas.Count;

        for (int i = 0; i < dropCount; i++)
        {
            if (_currentCardDatas.Count == 0) return;

            Card dropCard = tableTop.Current_DraggingCard();
            if (dropCard == null) continue;

            Card_Movement dropCardMovement = dropCard.movement;

            Vector2 dropPos = dropPositions.Count > 0 ? dropPositions[0] : cursorPos;
            dropPositions.RemoveAt(0);

            dropCardMovement.Toggle_DragDrop(false);
            dropCardMovement.Update_Shadows();

            dropCard.Update_Visuals();
            dropCard.Update_StackCards();

            dropCardMovement.Assign_TargetPosition(dropPos);
            dropCardMovement.Push_OverlappedCards();

            DragUpdate_CurrentCard();
        }
    }

    public void DragUpdate_CurrentCard()
    {
        TableTop tableTop = Game_Controller.instance.tableTop;

        if (tableTop.Current_DraggingCard() != null) return;
        if (_currentCardDatas.Count == 0) return;

        Card_Data recentData = _currentCardDatas[_currentCardDatas.Count - 1];
        if (recentData == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 spawnPos = _camera.ScreenToWorldPoint(mousePos);

        GameObject emptyCard = Instantiate(_emptyCardPrefab, spawnPos, Quaternion.identity);
        emptyCard.transform.SetParent(tableTop.allCards);

        if (emptyCard.TryGetComponent(out Card dragCard) == false) return;

        tableTop.Track_CurrentCard(dragCard);
        dragCard.Set_Data(recentData);

        Card_Movement movement = dragCard.movement;

        movement.Toggle_DragDrop(true);
        movement.Update_Shadows();

        dragCard.Update_Visuals();
        dragCard.Update_StackCards();
    }


    // Current Cards Visual
    public void Toggle_DragCardCount()
    {
        bool toggle = Input_Controller.instance.isIdle && _currentCardDatas.Count > 1;
        _dragCardCountBox.gameObject.SetActive(toggle);

        if (!toggle) return;

        Update_CursorPoint();
        _dragCardCountText.text = _currentCardDatas.Count.ToString();
    }

    public void Update_HoverCardInfo(Card hoveringCard)
    {
        _cardDescription.gameObject.SetActive(hoveringCard != null);
        if (hoveringCard == null) return;
        
        Card_Data data = hoveringCard.data;
        cardDescriptionText.text = data.cardScrObj.cardName;
    }
}
