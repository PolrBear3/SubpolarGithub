using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelStatus_Indicator : MonoBehaviour
{
    [Space(20)] 
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _timeText;

    [Space(20)] 
    [SerializeField] private Sprite[] _iconSprites;
    
    
    private float _defaultXPosition;
    
    
    // MonoBehaviour
    private void Awake()
    {
        _defaultXPosition = _iconImage.rectTransform.anchoredPosition.x;
    }


    // Main
    public void Toggle_Lock(bool toggle)
    {
        _timeText.gameObject.SetActive(!toggle);
        
        Vector2 imagePosition = toggle ? Vector2.zero : new Vector2(_defaultXPosition, 0f);
        _iconImage.rectTransform.anchoredPosition = imagePosition;
        
        _iconImage.sprite = toggle ? _iconSprites[0] : _iconSprites[1];
    }

    public void Set_Time(float timeValue)
    {
        int seconds = Mathf.FloorToInt(timeValue);
        int hundredths = Mathf.FloorToInt((timeValue - seconds) * 100f);

        string formatted = $"{seconds:D2}.{hundredths:D2}";
        _timeText.text = formatted;
    }
}
