using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CardMenu_Slot : MonoBehaviour
{
    [Space(20)]
    [SerializeField] private Image _baseImage;
    [SerializeField] private Image _cardIconImage;

    [Space(20)]
    [SerializeField] private Image _amountPanel;
    [SerializeField] private TextMeshProUGUI _amountText;

    [Space(20)]
    [SerializeField][Range(0, 1)] private float _lockTransparency;

    [SerializeField][Range(0, 100)] private float _highlightScaleValue;
    [SerializeField][Range(0, 10)] private float _highlightDuration;


    private Card_ScrObj _currentCard;
    private bool _highlighted;


    // Data Control
    public void Empty_Slot()
    {
        _currentCard = null;
       
        _baseImage.color = Color.clear;
        _cardIconImage.color = Color.clear;
    }

    public void Update_Slot(Card_ScrObj updateCard)
    {
        _currentCard = updateCard;

        if (_currentCard == null)
        {
            Empty_Slot();
            return;
        }
        
        _cardIconImage.sprite = updateCard.iconSprite;

        _baseImage.color = Color.white;
        _cardIconImage.color = Color.white;
    }


    // Toggle
    public void Toggle_LockIndication(bool toggle)
    {
        if (_currentCard == null) return;
        
        float lockAlpha = toggle ? _lockTransparency : 1f;

        Color baseColor = _baseImage.color;
        baseColor.a = lockAlpha;

        _baseImage.color = baseColor;

        Color iconColor = _cardIconImage.color;
        iconColor.a = lockAlpha;

        _cardIconImage.color = iconColor;
    }

    public void Toggle_Highlight(bool toggle)
    {
        if (toggle == _highlighted) return;
        _highlighted = toggle;

        float scaleValue = _highlighted ? _highlightScaleValue : -_highlightScaleValue;

        RectTransform baseRect = _baseImage.rectTransform;
        Vector2 baseSize = new(baseRect.rect.width + scaleValue, baseRect.rect.height + scaleValue);

        LeanTween.size(baseRect, baseSize, _highlightDuration);

        RectTransform iconRect = _cardIconImage.rectTransform;
        Vector2 iconSize = new(iconRect.rect.width + scaleValue, iconRect.rect.height + scaleValue);

        LeanTween.size(iconRect, iconSize, _highlightDuration);
    }
    public void Toggle_Highlight()
    {
        Toggle_Highlight(!_highlighted);
    }

    public void Update_AmountIndication(int updateValue)
    {
        bool toggle = updateValue > 0;
        _amountPanel.gameObject.SetActive(toggle);

        if (toggle == false) return;
        _amountText.text = updateValue.ToString(); 
    }
}