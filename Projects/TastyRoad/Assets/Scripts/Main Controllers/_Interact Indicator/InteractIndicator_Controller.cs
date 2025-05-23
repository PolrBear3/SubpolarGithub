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
    [SerializeField] private GameObject _iconBox;
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
        Input_Controller input = Input_Controller.instance;
        
        input.OnActionMapUpdate += () => _infoBox.gameObject.SetActive(false);
        input.OnActionMapUpdate += () => _iconBox.SetActive(input.Current_ActionMapNum() == 0 && _iconImage.sprite != null);
    }
    
    
    // Trigger
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
    }

    private void Toggle_InfoBox()
    {
        Cancel_Coroutine();
        _coroutine = StartCoroutine(Show_InfoBox());
    }
    private IEnumerator Show_InfoBox()
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
}