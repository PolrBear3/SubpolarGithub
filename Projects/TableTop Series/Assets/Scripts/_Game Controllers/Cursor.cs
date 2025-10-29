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

    [SerializeField] private TextMeshProUGUI cardNameText;
    [SerializeField] private TextMeshProUGUI cardDescriptionText;

    [Space(20)] 
    [SerializeField] private RectTransform _dragCardCountBox;
    [SerializeField] private TextMeshProUGUI _dragCardCountText;

    [Space(20)]
    [SerializeField] private RectTransform _cardDropPoint;


    private List<Card_Data> _currentCardDatas = new();
    public List<Card_Data> currentCardDatas => _currentCardDatas;

    private Card _recentDragCard;


    private float _pivotUpdatePositionY;
    private bool _cursorPointActive;


    // MonoBehaviour
    private void Start()
    {
        Game_Controller controller = Game_Controller.instance;
        _camera = controller.mainCamera;

        _pivotUpdatePositionY = _cardDescription.anchoredPosition.y;

        Toggle_DragCardCount();
        Update_CardDescriptions(null);

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
    private void Update_CursorPoint()
    {
        _uiCursorPoint.position = Mouse.current.position.ReadValue();
    }

    public Vector2 Mouse_WorldPoint()
    {
        return _camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
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
        currentCard.movement.Update_Shadows();
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

            Card_Movement movement = dropCard.movement;

            Vector2 dropPos = dropPositions.Count > 0 ? dropPositions[0] : cursorPos;
            dropPositions.RemoveAt(0);

            movement.Toggle_DragDrop(false);

            dropCard.Update_Visuals();
            dropCard.Update_StackCards();
            movement.Update_Shadows();

            movement.Assign_TargetPosition(dropPos);
            movement.Push_OverlappedCards();

            DragUpdate_CurrentCard();
        }
    }

    public void DragUpdate_CurrentCard()
    {
        TableTop tableTop = Game_Controller.instance.tableTop;
        Card draggingCard = tableTop.Current_DraggingCard();

        // remove current drag card if data does not exist
        if (draggingCard != null && _currentCardDatas.Contains(draggingCard.data) == false)
        {
            tableTop.currentCards.Remove(draggingCard);
            Destroy(draggingCard.gameObject);

            return;
        }

        if (draggingCard != null) return;
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

        dragCard.Update_Visuals();
        dragCard.Update_StackCards();

        Vector2 initialShadowPos = movement.shadowOffset + dragCard.StackOffset(_currentCardDatas.Count + 1);

        movement.Update_Shadows(initialShadowPos);
        movement.Update_Shadows();
    }


    // Cursor Point Indicators
    public void Toggle_DragCardCount()
    {
        bool toggle = Input_Controller.instance.isIdle && _currentCardDatas.Count > 1;
        _dragCardCountBox.gameObject.SetActive(toggle);

        if (!toggle) return;

        Update_CursorPoint();
        _dragCardCountText.text = _currentCardDatas.Count.ToString();
    }


    private void Update_CardDescriptions(Card updateCard)
    {
        _cardDescription.gameObject.SetActive(updateCard != null);
        if (updateCard == null) return;

        Card_ScrObj cardScrObj = updateCard.data.cardScrObj;
        if (cardScrObj == null) return;

        cardNameText.text = cardScrObj.cardName;
        cardDescriptionText.text = cardScrObj.cardName; // add descriptions data, update the data !
    }
    public void Update_CardDescriptions()
    {
        TableTop tableTop = Game_Controller.instance.tableTop;

        Card currentHoverCard = tableTop.Current_DraggingCard() == null ? tableTop.Current_HoverCard() : null;
        if (currentHoverCard != null && _cardDescription.gameObject.activeSelf) return;

        Update_CardDescriptions_Position(currentHoverCard);
        Update_CardDescriptions(currentHoverCard);
    }

    public void UnToggle_CardDescriptions()
    {
        _cardDescription.gameObject.SetActive(false);
    }

    private void Update_CardDescriptions_Position(Card updateCard)
    {
        if (updateCard == null) return;

        Update_CursorPoint(updateCard.transform.position);

        Vector2 mousePoint = Mouse_WorldPoint();
        bool flip = mousePoint.x > 0 ? true : false;

        float posX = Mathf.Abs(_cardDescription.anchoredPosition.x);
        posX = flip ? -posX : posX;

        float pivotY = mousePoint.y > 0f ? 1f : 0f;
        _cardDescription.pivot = new(_cardDescription.pivot.x, pivotY);

        float posY = pivotY == 1f ? _pivotUpdatePositionY : -_pivotUpdatePositionY;
        _cardDescription.anchoredPosition = new(posX, posY);
    }
}
