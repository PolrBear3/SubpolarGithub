using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;

public class InformationBox : MonoBehaviour
{
    private RectTransform _rect;

    
    [Header("")]
    [SerializeField] private Image _boxImage;
    public Image boxImage => _boxImage;

    [SerializeField] private TextMeshProUGUI _infoText;

    [Header("")]
    [SerializeField] private float _heightIncreaseValue;

    [Header("")]
    [SerializeField] private InfoTemplate_Trigger _templateTrigger;
    public InfoTemplate_Trigger templateTrigger => _templateTrigger;
    
    
    private float _defaultHeight;
    private bool _defaultHeightSaved;

    private bool _flipped;
    public bool flipped => _flipped;


    // UnityEngine
    private void Awake()
    {
        _rect = gameObject.GetComponent<RectTransform>();
    }

    private void Start()
    {
        Set_DefalutHeight();
        gameObject.SetActive(false);
        
        Input_Controller input = Input_Controller.instance;
        input.Update_EmojiAsset(_infoText);
        
        // subscriptions
        input.OnSchemeUpdate += () => input.Update_EmojiAsset(_infoText);
    }

    private void OnDestroy()
    {
        // subscriptions
        Input_Controller input = Input_Controller.instance;
        input.OnSchemeUpdate -= () => input.Update_EmojiAsset(_infoText);
    }


    // Data Set
    public void Set_DefalutHeight()
    {
        if (_defaultHeightSaved == true) return;

        _defaultHeight = _rect.anchoredPosition.y;
        _defaultHeightSaved = true;
    }


    // Panel Layout Control
    public void Flip()
    {
        _flipped = !_flipped;

        float currentX = _rect.anchoredPosition.x;
        _rect.anchoredPosition = new Vector2(currentX * -1, _rect.anchoredPosition.y);
    }
    public void Flip_toDefault()
    {
        if (_flipped == false) return;

        Flip();
    }


    public void Update_InfoText(string infoText)
    {
        _infoText.text = infoText.ToString();
    }

    public void Update_RectLayout()
    {
        _infoText.ForceMeshUpdate();
        LayoutRebuilder.ForceRebuildLayoutImmediate(_infoText.rectTransform);

        float lineCount = _infoText.textInfo.lineCount;
        float updateValue = _heightIncreaseValue * lineCount;
        float targetPosY = _defaultHeight + _heightIncreaseValue - updateValue;

        _rect.anchoredPosition = new Vector2(_rect.anchoredPosition.x, targetPosY);
    }
}
