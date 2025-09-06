using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Cursor : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    public RectTransform rectTransform => _rectTransform;
    
    [SerializeField] private Image _image;
    public Image image => _image;

    
    private bool _isActive;
    public Action<int> OnLayerClick;


    // MonoBehaviour
    private void Start()
    {
        _image.color = Color.clear;
        
        // subscriptions
        Input_Controller.instance.OnClick += OnClick;
    }

    private void OnDestroy()
    {
        // subscriptions
        Input_Controller.instance.OnClick -= OnClick;
    }

    private void Update()
    {
        if (_isActive == false) return;
        _rectTransform.position = Input.mousePosition;
    }
    
    
    // Main
    public void Toggle_Activation(bool toggle)
    {
        _isActive = toggle;
    }

    private void OnClick()
    {
        if (_isActive == false) return;

        Vector2 screenPos = Mouse.current != null ? Mouse.current.position.ReadValue() : (Vector2)Input.mousePosition;
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

        Collider2D hoveringCollider = Physics2D.OverlapPoint(worldPos);

        if (hoveringCollider == null)
        {
            OnLayerClick?.Invoke(0);
            return;
        }
        OnLayerClick?.Invoke(hoveringCollider.gameObject.layer);
    }
}
