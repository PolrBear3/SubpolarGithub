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

    private bool _cursorPointActive;


    // MonoBehaviour
    private void Start()
    {
        _camera = Game_Controller.instance.mainCamera;

        Toggle_DragCardCount();

        // subscriptions
        Input_Controller input = Input_Controller.instance;

        input.OnMultiSelect += Drag_MultipleCards;
        input.OnMultiSelect += DropAll_CurrentCards;
    }

    private void OnDestroy()
    {
        // subscriptions
        Input_Controller input = Input_Controller.instance;

        input.OnMultiSelect -= Drag_MultipleCards;
        input.OnMultiSelect -= DropAll_CurrentCards;
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
        if (overlappedCards.Count == 0) return;

        Card dragTargetCard = overlappedCards[0];
        _currentCardDatas.Add(dragTargetCard.data);

        tableTop.currentCards.Remove(dragTargetCard);
        Destroy(dragTargetCard.gameObject);
    }
    private void DropAll_CurrentCards()
    {
        if (_currentCardDatas.Count == 0) return;

        // ?
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
        dragCard.Set_Data(recentData);

        Card_Movement movement = dragCard.movement;

        movement.Toggle_DragDrop(true);
        movement.Update_Shadows();

        dragCard.Update_Visuals();
        dragCard.Update_LayerOrder();
    }


    // Current Cards Visual
    public void Toggle_DragCardCount()
    {
        bool toggle = _currentCardDatas.Count > 1;
        _dragCardCountBox.gameObject.SetActive(toggle);

        if (!toggle) return;
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
