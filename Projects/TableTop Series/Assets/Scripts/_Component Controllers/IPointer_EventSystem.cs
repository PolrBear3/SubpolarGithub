using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class IPointer_EventSystem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Action OnEnter;
    public Action OnEnterDelay;
    
    public Action OnPoint;
    public Action OnIdle;
    
    public Action OnExit;
    
    public Action OnSelect;
    public Action OnMultiSelect;


    private Coroutine _enterDelayCoroutine;

    private bool _pointerEntered;
    public bool pointerEntered => _pointerEntered;
    
    
    // MonoBehaviour
    private void Start()
    {
        Input_Controller input = Input_Controller.instance;

        input.OnPoint += OnPointer_Point;
        input.OnIdle += OnPointer_Idle;
        
        input.OnSelect += OnPointer_Select;
        input.OnMultiSelect += OnPointer_MultiSelect;
    }

    private void OnDestroy()
    {
        Input_Controller input = Input_Controller.instance;
        
        input.OnPoint -= OnPointer_Point;
        input.OnIdle -= OnPointer_Idle;
        
        input.OnSelect -= OnPointer_Select;
        input.OnMultiSelect -= OnPointer_MultiSelect;
    }


    // EventSystems
    public void OnPointerEnter(PointerEventData eventData)
    {
        _pointerEntered = true;
        OnEnter?.Invoke();

        if (_enterDelayCoroutine != null) StopCoroutine(_enterDelayCoroutine);
        _enterDelayCoroutine = StartCoroutine(EnterDelay_Coroutine());
    }
    private IEnumerator EnterDelay_Coroutine()
    {
        float delayTime = Input_Controller.instance.idleThreshold;
        yield return new WaitForSeconds(delayTime);

        OnEnterDelay?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _pointerEntered = false;
        OnExit?.Invoke();

        if (_enterDelayCoroutine == null) return;
        StopCoroutine(_enterDelayCoroutine);
    }


    private void OnPointer_Idle()
    {
        if (!_pointerEntered) return;
        OnIdle?.Invoke();
    }

    private void OnPointer_Point()
    {
        OnPoint?.Invoke();
    }
    
    
    public void OnPointer_Select()
    {
        if (!_pointerEntered) return;
        OnSelect?.Invoke();

        if (_enterDelayCoroutine != null) StopCoroutine(_enterDelayCoroutine);
        _enterDelayCoroutine = StartCoroutine(EnterDelay_Coroutine());
    }

    public void OnPointer_MultiSelect()
    {
        if (!_pointerEntered) return;
        OnMultiSelect?.Invoke();

        if (_enterDelayCoroutine != null) StopCoroutine(_enterDelayCoroutine);
        _enterDelayCoroutine = StartCoroutine(EnterDelay_Coroutine());
    }
}
