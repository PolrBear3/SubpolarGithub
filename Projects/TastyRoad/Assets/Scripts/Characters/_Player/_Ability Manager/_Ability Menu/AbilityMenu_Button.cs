using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityMenu_Button : MonoBehaviour
{
    [Space(20)] 
    [SerializeField] private RectTransform _infoBoxRect;
    [SerializeField] private RectTransform _buttonShadowRect;
    
    [Space(20)] 
    [SerializeField] private Image _iconImage;
    [SerializeField] private TMP_Text _infoText;

    [Space(20)] 
    [SerializeField] private UI_ClockTimer _holdTimer;
    public UI_ClockTimer holdTimer => _holdTimer;
    
    [SerializeField] private GameObject _selectIndicator;
    public GameObject selectIndicator => _selectIndicator;


    private Vector2 _defaultPosition;
    public Vector2 defaultPosition => _defaultPosition;
    
    private Ability_ScrObj _abilityScrObj;
    public Ability_ScrObj abilityScrObj => _abilityScrObj;
    
    
    // MonoBehaviour
    private void Awake()
    {
        _defaultPosition = _infoBoxRect.anchoredPosition;
    }
    
    
    // Toggle
    public void Toggle_Select(bool toggle)
    {
        _selectIndicator.SetActive(toggle);

        if (toggle == false)
        {
            _infoBoxRect.anchoredPosition = _defaultPosition;
            return;
        }
        _infoBoxRect.anchoredPosition = _buttonShadowRect.anchoredPosition;
    }
    
    
    // Indication
    public void Set_AbilityIndication(Ability activatedAbility)
    {
        Ability_ScrObj abilityScrObj = activatedAbility.abilityScrObj;
        int activationCount = activatedAbility.activationCount;
        
        _abilityScrObj = abilityScrObj;
        
        _iconImage.sprite = abilityScrObj.ProgressIcon(activationCount + 1);
        _infoText.text = abilityScrObj.Description();
    }

    public void Empty_AbilityIndication(Sprite emptyIconSprite, string emptyDescription)
    {
        _iconImage.sprite = emptyIconSprite;
        _infoText.text = emptyDescription;
    }
}
