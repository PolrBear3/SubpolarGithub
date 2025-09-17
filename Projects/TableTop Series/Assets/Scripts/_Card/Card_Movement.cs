using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Movement : MonoBehaviour
{
    [Space(20)] 
    [SerializeField] private SpriteRenderer _cardShadow;
    
    [Space(20)] 
    [SerializeField] private Vector2 _shadowOffset;
    [SerializeField][Range(0, 100)] private float _moveSpeed;

    private bool _dragging;
    public bool dragging => _dragging;
    

    // MonoBehaviour
    private void Start()
    {
        Update_Shadows();
    }

    private void Update()
    {
        MouseFollow_Update();
    }


    // Main
    public void Toggle_DragDrop()
    {
        _dragging = !_dragging;
    }
    
    private void MouseFollow_Update()
    {
        if (_dragging == false) return;
        
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.WorldToScreenPoint(transform.position).z; 
        
        Vector3 targetPos = Camera.main.ScreenToWorldPoint(mousePos);
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * _moveSpeed);
    }

    
    // Effects
    public void Update_Shadows()
    {
        _cardShadow.transform.localPosition = _dragging ? _shadowOffset : Vector2.zero;
    }
}