using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Object_Scanner : MonoBehaviour
{
    private Drag_Slot _dragSlot;

    [SerializeField] private Image _droppedIcon; 
    [SerializeField] private Text _droppedText;

    private bool _objectDetected;
    public bool objectDetected { get => _objectDetected; set => _objectDetected = value; }

    private void Awake()
    {
        if (gameObject.TryGetComponent(out Drag_Slot dragSlot)) { _dragSlot = dragSlot; }
    }

    public void Show_Amount(int amount)
    {
        if (_dragSlot.itemDragging) return;

        _objectDetected = true;

        _droppedIcon.color = Color.white;

        Color textColor = _droppedText.color;

        _droppedText.text = amount.ToString();
        textColor.a = 1f;
        _droppedText.color = textColor;
    }
    public void Hide_Amount()
    {
        _objectDetected = false;

        _droppedIcon.color = Color.clear;

        Color textColor = _droppedText.color;

        textColor.a = 0f;
        _droppedText.color = textColor;
    }
}
