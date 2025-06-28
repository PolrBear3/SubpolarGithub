using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [Space(20)]
    [SerializeField] private Transform _movementTransform;
    
    [Space(10)]
    [SerializeField][Range(0, 10)] private float _movementSpeed;
    [SerializeField] private LeanTweenType _tweenType;


    private Vector2 _defaultPosition;
    private Vector2 _movementPosition;
    
    
    // MonoBehaviour
    private void Awake()
    {
        _defaultPosition = transform.position;
        _movementPosition = _movementTransform.position;
    }
    
    
    // Movement
    public void Cancel_Movement()
    {
        LeanTween.cancel(gameObject);
    }
    
    public void Toggle_Movement(bool toggle)
    {
        Cancel_Movement();

        Vector2 targetPosition = toggle ? _movementPosition : _defaultPosition;
        
        float distance = Vector3.Distance(transform.position, targetPosition);
        float duration = distance / Mathf.Max(_movementSpeed, 0.01f);

        LeanTween.move(gameObject, targetPosition, duration).setEase(_tweenType);
    }
}
