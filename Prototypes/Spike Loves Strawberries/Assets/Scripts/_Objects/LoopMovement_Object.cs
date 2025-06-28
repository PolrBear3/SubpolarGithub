using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopMovement_Object : MonoBehaviour
{
    [Space(20)]
    [SerializeField] private Transform _movementTransform;
    
    [Space(10)]
    [SerializeField][Range(0, 10)] private float _movementSpeed;
    [SerializeField] private LeanTweenType _tweenType;

    [Space(10)]
    [SerializeField] private bool _toggled;
    [SerializeField] [Range(0, 100)] private float _loopDelayTime;


    private bool _isReturning;
    
    private Vector2 _defaultPosition;
    private Vector2 _movementPosition;

    private Coroutine _coroutine;
    
    
    // MonoBehaviour
    public void Awake()
    {
        _defaultPosition = transform.position;
        _movementPosition = _movementTransform.position;
    }
    
    public void Start()
    {
        Toggle_Movement(_toggled);
    }


    // Main
    public void Cancel_Movement()
    {
        LeanTween.cancel(gameObject);

        if (_coroutine == null) return;
        
        StopCoroutine(_coroutine);
        _coroutine = null;
    }
    
    public void Toggle_Movement(bool toggle)
    {
        Cancel_Movement();
        
        if (toggle == false) return;
        StartCoroutine(Movement_Coroutine());
    }
    private IEnumerator Movement_Coroutine()
    {
        while (_toggled)
        {
            Vector2 targetPosition = _isReturning ? _defaultPosition : _movementPosition;
        
            float distance = Vector3.Distance(transform.position, targetPosition);
            float duration = distance / Mathf.Max(_movementSpeed, 0.01f);

            LeanTween.move(gameObject, targetPosition, duration).setEase(_tweenType);
            
            yield return new WaitForSeconds(duration);
            _isReturning = !_isReturning;
            
            yield return new WaitForSeconds(_loopDelayTime);
        }
    }
}
