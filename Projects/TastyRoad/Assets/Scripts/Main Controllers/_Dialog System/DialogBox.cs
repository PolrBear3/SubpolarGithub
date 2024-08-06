using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogBox : MonoBehaviour
{
    [Header("")]
    [SerializeField] private Image _iconImage;
    [SerializeField] private RectTransform _newIndication;


    // Data Set
    public void Update_Box(DialogData data)
    {
        _iconImage.sprite = data.icon;
    }

    public void UpdateIcon_CenterPosition(Vector2 centerPosition)
    {
        _iconImage.rectTransform.localPosition = centerPosition;
    }
}
