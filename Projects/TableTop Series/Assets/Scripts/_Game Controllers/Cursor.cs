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
    
    [Space(20)] 
    [SerializeField] private RectTransform _cardDescriptionBox;
    [SerializeField] private TextMeshProUGUI _cardNameText;

    [Space(20)] 
    [SerializeField] private RectTransform _stackAmountBox;
    [SerializeField] private TextMeshProUGUI _stackAmountText;
    
    
    private List<Card> _currentCards = new();
    public List<Card> currentCards => _currentCards;

    private bool _cursorPointActive;


    // MonoBehaviour
    private void Start()
    {
        _camera = Game_Controller.instance.mainCamera;

        Update_HoverCardInfo(null);
    }

    private void Update()
    {
        CursorPoint_Update();
    }


    // Data
    public Card Recent_CurrentCard()
    {
        if (_currentCards.Count <= 0) return null;
        return _currentCards[_currentCards.Count - 1];
    }
    
    
    // Main
    private void CursorPoint_Update()
    {
        if (!_cursorPointActive) return;
        
        _uiCursorPoint.position = Mouse.current.position.ReadValue();
    }
    
    public void Toggle_CursorPoint(bool toggle)
    {
        _cursorPointActive = toggle;
    }
    
    
    // Hover Card Info
    public void Update_HoverCardInfo(Card hoveringCard)
    {
        _cardDescriptionBox.gameObject.SetActive(hoveringCard != null);
        if (hoveringCard == null) return;
        
        Card_Data data = hoveringCard.data;
        int currentCardsCount = _currentCards.Count;
        
        _cardNameText.text = data.cardScrObj.cardName;
        
        _stackAmountBox.gameObject.SetActive(currentCardsCount > 1);
        if (currentCardsCount <= 1) return;
        
        _stackAmountText.text = currentCardsCount.ToString();
    }
}
