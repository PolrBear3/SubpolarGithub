using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityMenu_Button : MonoBehaviour
{
    [Space(20)] 
    [SerializeField] private Image _iconImage;
    [SerializeField] private TMP_Text _infoText;
    
    [Space(20)] 
    [SerializeField] private GameObject _selectIndicator;
    public GameObject selectIndicator => _selectIndicator;


    private Ability_ScrObj _abilityScrObj;
    public Ability_ScrObj abilityScrObj => _abilityScrObj;
    
    
    // Indication
    public void Set_AbilityIndication(Ability activatedAbility)
    {
        Ability_ScrObj abilityScrObj = activatedAbility.abilityScrObj;
        int activationCount = activatedAbility.activationCount;
        
        _abilityScrObj = abilityScrObj;
        
        _iconImage.sprite = abilityScrObj.ProgressIcon(activationCount);
        _infoText.text = abilityScrObj.Description();
    }

    public void Empty_AbilityIndication(Sprite emptyIconSprite, string emptyDescription)
    {
        _iconImage.sprite = emptyIconSprite;
        _infoText.text = emptyDescription;
    }
}
