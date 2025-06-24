using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [Space(20)] 
    [SerializeField] private SpriteRenderer _sr;
    [SerializeField] private Sprite[] _plateSprites;
    
    [Space(20)] 
    [SerializeField] private Detection_Controller _detection;

    [Space(20)] 
    [SerializeField][Range(0, 10)] private float _updateTime;


    private Coroutine _coroutine;
    private int _spriteIndex;

    public Action OnPress;
    public Action OnRelease;
    
    
    // MonoBehaviour
    private void Start()
    {
        _detection.OnPlayerDetect += Update_PlatePressure;
        _detection.OnPlayerExit += Update_PlatePressure;
    }

    private void OnDestroy()
    {
        _detection.OnPlayerDetect -= Update_PlatePressure;
        _detection.OnPlayerExit -= Update_PlatePressure;
    }
    
    
    // Interaction
    private void Update_PlatePressure()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }

        _coroutine = StartCoroutine(Update_PlatePressure_Coroutine(_detection.playerDetected));
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
            
            _coroutine = null;
            yield break;
        }

        while (_spriteIndex > 0)
        {
            _spriteIndex--;
            _sr.sprite = _plateSprites[_spriteIndex];
            
            yield return new WaitForSeconds(tikTime);
        }
        OnRelease?.Invoke();
        
        _coroutine = null;
        yield break;
    }
}
