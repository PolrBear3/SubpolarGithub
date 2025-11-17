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
    [SerializeField] private Sprite[] _amountPanelSprites;
    [SerializeField] private Image _amountPanel;
    [SerializeField] private TextMeshProUGUI _amountText;

    [Space(20)]
    [SerializeField][Range(0, 1)] private float _transparencyValue;

    [SerializeField][Range(0, 100)] private float _highlightScaleValue;
    [SerializeField][Range(0, 10)] private float _highlightDuration;


    private Card_ScrObj _currentCard;
    public Card_ScrObj currentCard => _currentCard;
    
    private bool _selected;
    public bool selected => _selected;


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
    public void Toggle_Transparency(bool toggle)
    {
        if (_currentCard == null) return;
        
        float lockAlpha = toggle ? _transparencyValue : 1f;

        Color baseColor = _baseImage.color;
        baseColor.a = lockAlpha;

        _baseImage.color = baseColor;

        Color iconColor = _cardIconImage.color;
        iconColor.a = lockAlpha;

        _cardIconImage.color = iconColor;
    }

    public void Toggle_SelectHighlight(bool toggle)
    {
        if (toggle == _selected) return;
        _selected = toggle;

        float scaleValue = _selected ? _highlightScaleValue : -_highlightScaleValue;

        RectTransform baseRect = _baseImage.rectTransform;
        Vector2 baseSize = new(baseRect.rect.width + scaleValue, baseRect.rect.height + scaleValue);

        LeanTween.size(baseRect, baseSize, _highlightDuration);

        RectTransform iconRect = _cardIconImage.rectTransform;
        Vector2 iconSize = new(iconRect.rect.width + scaleValue, iconRect.rect.height + scaleValue);

        LeanTween.size(iconRect, iconSize, _highlightDuration);
    }
    public void Toggle_SelectHighlight()
    {
        Toggle_SelectHighlight(!_selected);
    }


    // Amount Panel
    public void Update_AmountIndication(int updateValue)
    {
        bool toggle = updateValue > 0;
        _amountPanel.gameObject.SetActive(toggle);

        if (toggle == false) return;
        _amountText.text = updateValue.ToString(); 
    }
    public void Update_AmountIndication(int updateValue, int currentValue)
    {
        bool toggle = updateValue > 0;
        _amountPanel.gameObject.SetActive(toggle);

        if (toggle == false) return;
        _amountText.text = currentValue + "/" + updateValue;
    }

    public void Update_AmountPanel(bool toggleGreen)
    {
        Sprite panelSprite = toggleGreen ? _amountPanelSprites[2] : _amountPanelSprites[1];
        
        _amountPanel.sprite = panelSprite;
    }
    public void Update_AmountPanel()
    {
        _amountPanel.sprite = _amountPanelSprites[0];
    }
}