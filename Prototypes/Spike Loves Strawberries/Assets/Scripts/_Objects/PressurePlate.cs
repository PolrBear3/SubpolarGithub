using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    [Space(20)] 
    [SerializeField] private SpriteRenderer _sr;
    [SerializeField] private Sprite[] _plateSprites;
    
    [Space(20)] 
    [SerializeField] private Detection_Controller _detection;
    public Detection_Controller detection => _detection;

    [SerializeField][Range(0, 10)] private float _updateTime;

    [Space(20)] 
    [SerializeField] private UnityEvent OnPress;
    [SerializeField] private UnityEvent OnRelease;

    public Action onPressAction;
    public Action onReleaseAction;

    private Coroutine _coroutine;
    private int _spriteIndex;
    
    
    // MonoBehaviour
    private void Start()
    {
        // subscriptions
        _detection.OnPlayerDetect += Update_PlatePressure;
        _detection.OnPlayerExit += Update_PlatePressure;
        
        _detection.OnObjectDetect += Update_PlatePressure;
        _detection.OnObjectExit += Update_PlatePressure;

        Level_Controller.instance.OnLevelUpdate += OnDestroy;
    }

    private void OnDestroy()
    {
        // subscriptions
        _detection.OnPlayerDetect -= Update_PlatePressure;
        _detection.OnPlayerExit -= Update_PlatePressure;
        
        _detection.OnObjectDetect -= Update_PlatePressure;
        _detection.OnObjectExit -= Update_PlatePressure;
        
        Level_Controller.instance.OnLevelUpdate -= OnDestroy;
    }
    
    
    // Interaction
    private void Update_PlatePressure()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }

        bool isPressed = _detection.playerDetected || PickupObject_Placed();
        _coroutine = StartCoroutine(Update_PlatePressure_Coroutine(isPressed));
    }
    private IEnumerator Update_PlatePressure_Coroutine(bool isPressed)
    {
        float tikTime = _updateTime / _plateSprites.Length;
        
        if (isPressed)
        {
            while (_spriteIndex < _plateSprites.Length - 1)
            {
                _spriteIndex++;
                _sr.sprite = _plateSprites[_spriteIndex];
                
                yield return new WaitForSeconds(tikTime);
            }
            OnPress?.Invoke();
            onPressAction?.Invoke();
            
            _coroutine = null;
            yield break;
        }
        
        OnRelease?.Invoke();
        onReleaseAction?.Invoke();

        while (_spriteIndex > 0)
        {
            _spriteIndex--;
            _sr.sprite = _plateSprites[_spriteIndex];
            
            yield return new WaitForSeconds(tikTime);
        }
        
        _coroutine = null;
        yield break;
    }

    private bool PickupObject_Placed()
    {
        List<GameObject> detectedObjects = _detection.detectedObjects;
        if (detectedObjects.Count <= 0) return false;

        for (int i = 0; i < detectedObjects.Count; i++)
        {
            if (detectedObjects[i].TryGetComponent(out Pickup_Object pickup) == false) continue;
            return true;
        }
        return false;
    }
}