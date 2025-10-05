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
    [SerializeField] private RectTransform _stackAmountBox;
    [SerializeField] private TextMeshProUGUI _stackAmountText;
    
    
    private List<Card_Data> _currentCardDatas = new();
    public List<Card_Data> currentCardDatas => _currentCardDatas;

    private bool _cursorPointActive;


    // MonoBehaviour
    private void Start()
    {
        _camera = Game_Controller.instance.mainCamera;

        Update_HoverCardInfo(null);

        // subscriptions
        Input_Controller.instance.OnMultiSelect += DragDrop_MultipleCards;
    }

    private void OnDestroy()
    {
        // subscriptions
        Input_Controller.instance.OnMultiSelect -= DragDrop_MultipleCards;
    }

    private void Update()
    {
        CursorPoint_Update();
    }


    // Main
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


    // Cards
    private void DragDrop_MultipleCards()
    {
        TableTop tableTop = Game_Controller.instance.tableTop;

        Card currentCard = tableTop.Current_DraggingCard();
        if (currentCard == null) return;

        List<Card> overlappedCards = currentCard.detection.Closest_DetectedCards();

        // drop all
        if (overlappedCards.Count == 0)
        {
            Debug.Log("Drop Actions for all current cards");
            return;
        }

        Card dragTargetCard = overlappedCards[0];
        _currentCardDatas.Add(dragTargetCard.data);

        tableTop.currentCards.Remove(dragTargetCard);
        Destroy(dragTargetCard.gameObject);
    }

    public void DragUpdate_CurrentCard()
    {
        if (Game_Controller.instance.tableTop.Current_DraggingCard() != null) return;
        if (_currentCardDatas.Count == 0) return;

        Card_Data recentData = _currentCardDatas[_currentCardDatas.Count - 1];
        if (recentData == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 spawnPos = _camera.ScreenToWorldPoint(mousePos);

        GameObject emptyCard = Instantiate(_emptyCardPrefab, spawnPos, Quaternion.identity);
        if (emptyCard.TryGetComponent(out Card dragCard) == false) return;

        dragCard.Set_Data(recentData);

        dragCard.Update_Visuals();
        dragCard.Update_LayerOrder();

        Card_Movement movement = dragCard.movement;

        movement.Toggle_DragDrop(true);
        movement.Update_Shadows();
    }


    public void Update_HoverCardInfo(Card hoveringCard)
    {
        _cardDescription.gameObject.SetActive(hoveringCard != null);
        if (hoveringCard == null) return;
        
        Card_Data data = hoveringCard.data;
        int currentCardsCount = _currentCardDatas.Count;
        
        cardDescriptionText.text = data.cardScrObj.cardName;
        
        _stackAmountBox.gameObject.SetActive(currentCardsCount > 1);
        if (currentCardsCount <= 1) return;
        
        _stackAmountText.text = currentCardsCount.ToString();
    }
}
