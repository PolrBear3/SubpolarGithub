using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogBox : MonoBehaviour
{
    private DialogData _data;
    public DialogData data => _data;

    [Header("")]
    [SerializeField] private Image _iconImage;
    public Image iconImage => _iconImage;

    [SerializeField] private RectTransform _infoBoxPosition;
    public RectTransform infoBoxPosition => _infoBoxPosition;


    // Data Set
    public void Set_Data(DialogData setData)
    {
        _data = setData;
    }


    // Updates
    public void Set_IconImage(Sprite icon)
    {
        _iconImage.sprite = icon;
    }
    
    public void Update_IconImage()
    {
        _iconImage.sprite = _data.icon;
    }
}
