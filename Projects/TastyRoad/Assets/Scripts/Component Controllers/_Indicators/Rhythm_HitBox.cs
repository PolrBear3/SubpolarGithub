using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class Rhythm_HitBox : MonoBehaviour
{
    [Header("")]
    [SerializeField] private ActionKey _actionKey;
    [SerializeField] private InputActionReference[] _actionRefs;
    
    [Header("")] 
    [SerializeField] private SpriteRenderer _hitBox;
    [SerializeField] private Sprite[] _boxSprites;
    
    [Header("")] 
    [SerializeField] private SpriteRenderer _resultBox;
    [SerializeField] private Sprite[] _resultSprites;


    private bool _toggled;
    private int _rhythmIndex;

    public Action OnHitSuccess;
    
    private Coroutine _activeCoroutine;


    // UnityEngine
    private void Start()
    {
        Toggle(false);
        _resultBox.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        Input_Controller.instance.OnAnyInput -= Hit;
    }


    // Activation Toggle
    public void Toggle(bool toggle)
    {
        _toggled = toggle;
        _hitBox.gameObject.SetActive(toggle);

        if (toggle == false)
        {
            Input_Controller.instance.OnAnyInput -= Hit;
            _actionKey.Reset_CurrentKey();

            if (_activeCoroutine == null) return;
            
            StopCoroutine(_activeCoroutine);
            _activeCoroutine = null;
            
            return;
        }

        if (_activeCoroutine != null) return;
        _activeCoroutine = StartCoroutine(Activate());
    }

    private IEnumerator Activate()
    {
        Input_Controller.instance.OnAnyInput += Hit;
        
        while (_toggled)
        {
            _actionKey.Reset_CurrentKey();
            
            InputActionReference randRef = _actionRefs[Random.Range(0, _actionRefs.Length)];
            _actionKey.Set_CurrentKey(randRef);

            for (int i = 0; i < _boxSprites.Length; i++)
            {
                _rhythmIndex = i;
                _hitBox.sprite = _boxSprites[i];
                
                yield return new WaitForSeconds(0.25f);
            }
        }

        Input_Controller.instance.OnAnyInput -= Hit;
        _actionKey.Reset_CurrentKey();
        
        _activeCoroutine = null;
        yield break;
    }
    
    private IEnumerator Update_SucessBox(bool success)
    {
        _resultBox.gameObject.SetActive(true);
        
        if (success)
        {
            _resultBox.sprite = _resultSprites[0];
        }
        else
        {
            _resultBox.sprite = _resultSprites[1];
        }
        
        yield return new WaitForSeconds(0.25f);
        _resultBox.gameObject.SetActive(false);

        yield break;
    }
    

    // Hit
    private bool Hit_Success(InputActionReference hitActionRef)
    {
        if (hitActionRef != _actionKey.currentReference) return false;
        if (_rhythmIndex != _boxSprites.Length - 1) return false;
        
        return true;
    }
    
    private void Hit(InputActionReference hitActionRef)
    {
        bool success = Hit_Success(hitActionRef);
        StartCoroutine(Update_SucessBox(success));
        
        if (success == false) return;
        OnHitSuccess?.Invoke();
    }
}
