using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IPointerClickHandler
{
    [Header("")]
    private Image _cardImage;
    [SerializeField] private Image _iconImage;

    private CardData _currentData;
    public CardData currentData => _currentData;


    // UnityEngine
    private void Awake()
    {
        _cardImage = gameObject.GetComponent<Image>();
    }


    // UnityEngine.EventSystems
    public void OnPointerClick(PointerEventData eventData)
    {
        
    }


    // Set Functions
    public void Set_CurrentData(CardData setData)
    {
        // current data set
        _currentData = setData;

        // card visual update
        _cardImage.sprite = setData.scrObj.cardSprite;

        if (setData.scrObj.iconSprite != null)
        {
            _iconImage.sprite = setData.scrObj.iconSprite;
            _iconImage.color = Color.white;
        }
        else _iconImage.color = Color.clear;
    }
}
