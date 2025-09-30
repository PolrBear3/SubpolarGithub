using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Cursor : MonoBehaviour
{
    private Camera _camera;
    
    private Card _currentCard;
    public Card currentCard => _currentCard;
    
    
    [Space(20)]
    [SerializeField] private RectTransform _uiCursorPoint;

    [Space(20)] 
    [SerializeField] private RectTransform _hoverCardInfo;
    [SerializeField] private TextMeshProUGUI _cardNameText;

    [Space(20)] 
    [SerializeField] private RectTransform _stackAmountBox;
    [SerializeField] private TextMeshProUGUI _stackAmountText;


    // MonoBehaviour
    private void Start()
    {
        _camera = Game_Controller.instance.mainCamera;

        Update_HoverCardInfo(null);
    }


    // Data
    public void Set_CurrentCard(Card setCard)
    {
        _currentCard = setCard;
    }
    
    
    // UI
    public void Update_PointPosition()
    {
        Vector2 mousePos = Input.mousePosition;
        _uiCursorPoint.position = mousePos;
    }
    
    
    // Hover Card Info
    public void Update_HoverCardInfo(Card hoveringCard)
    {
        _hoverCardInfo.gameObject.SetActive(hoveringCard != null);
        if (hoveringCard == null) return;
        
        Card_Data data = hoveringCard.data;
        int stackAmount = data.stackAmount;

        _cardNameText.text = data.cardScrObj.cardName;
        
        _stackAmountBox.gameObject.SetActive(stackAmount > 1);
        if (stackAmount <= 1) return;
        
        _stackAmountText.text = data.stackAmount.ToString();
    }
}
