using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractIndicator_Controller : MonoBehaviour
{
    public static InteractIndicator_Controller instance;


    [Space(20)] 
    [SerializeField] private UI_EffectController _uiEffect;
    [SerializeField] private RectTransform _interactIndicator;
    
    [Space(20)] 
    [SerializeField] private RectTransform _iconBox;
    [SerializeField] private Image _iconImage;

    [Space(20)] 
    [SerializeField] private Image _infoBox;
    [SerializeField] private TextMeshProUGUI _infoText;

    [Space(20)] 
    [SerializeField] [Range(0, 100)] private float _infoShowtime;

    
    private Coroutine _coroutine;
    
    
    // UnityEngine
    private void Awake()
    {
        instance = this;
        
        Trigger(null, null);
    }

    private void Start()
    {
        Toggle(Toggle_Available());
        
        // subscriptions
        Input_Controller.instance.OnActionMapUpdate += Toggle;
        Cutscene_Controller.instance.OnToggle += Toggle;
    }

    private void OnDestroy()
    {
        Cutscene_Controller.instance.OnToggle -= Toggle;
    }


    // Trigger
    private bool Toggle_Available()
    {
        bool inGame = Input_Controller.instance.Current_ActionMapNum() == 0 && _iconImage.sprite != null;
        bool cutscenePlaying = Cutscene_Controller.instance.coroutine != null;
        
        return inGame && cutscenePlaying == false;
    }
    
    private void Toggle(bool toggle)
    {
        if (toggle == false || _iconImage.sprite == null)
        {
            _iconBox.gameObject.SetActive(false);
            _infoBox.gameObject.SetActive(false);
            
            return;
        }
        Trigger(_iconImage.sprite, _infoText.text);
    }
    private void Toggle()
    {
        Toggle(Toggle_Available());
    }
    
    
    public void Trigger(Sprite icon, string info)
    {
        _iconBox.gameObject.SetActive(icon != null);

        if (icon == null)
        {
            _iconImage.sprite = null;
            
            _infoBox.gameObject.SetActive(false);
            return;
        }
        
        _iconImage.sprite = icon;
        _infoText.text = info;

        if (info == null || info == String.Empty)
        {
            _infoBox.gameObject.SetActive(false);
            return;
        }
        
        Toggle_InfoBox();
        
        _uiEffect.Update_Scale(_interactIndicator);
    }

    private void Toggle_InfoBox()
    {
        Cancel_Coroutine();
        _coroutine = StartCoroutine(InfoBox_Coroutine());
    }
    private IEnumerator InfoBox_Coroutine()
    {
        _infoBox.gameObject.SetActive(true);
       
        yield return new WaitForSeconds(_infoShowtime);
        
        _infoBox.gameObject.SetActive(false);
        _coroutine = null;
    }


    private void Cancel_Coroutine()
    {
        if (_coroutine == null) return;
        
        StopCoroutine(_coroutine);
        _coroutine = null;
    }
    
    
    // Action Bubble Trigger
}