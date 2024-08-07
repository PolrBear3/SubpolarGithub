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
    [SerializeField] private RectTransform _newIndication;


    // Data Set
    public void Set_Data(DialogData setData)
    {
        _data = setData;
    }


    // Updates
    public void Update_IconImage()
    {
        _iconImage.sprite = _data.icon;
    }

    public void UpdateIcon_CenterPosition(Vector2 centerPosition)
    {
        _iconImage.rectTransform.localPosition = centerPosition;
    }
}
