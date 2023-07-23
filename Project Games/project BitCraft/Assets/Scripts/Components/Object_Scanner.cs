using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Object_Scanner : MonoBehaviour
{
    private Drag_Slot _dragSlot;

    [SerializeField] private Text _amountText;

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

        Color textColor = _amountText.color;

        _amountText.text = amount.ToString();
        textColor.a = 1f;
        _amountText.color = textColor;
    }
    public void Hide_Amount()
    {
        _objectDetected = false;

        Color textColor = _amountText.color;

        textColor.a = 0f;
        _amountText.color = textColor;
    }
}
