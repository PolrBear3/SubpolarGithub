using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class IPointer_EventSystem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Action OnEnter;
    public Action OnExit;
    
    public Action OnSelect;
    public Action OnMultiSelect;

    private bool _pointerEntered;
    
    
    // MonoBehaviour
    private void Start()
    {
        Input_Controller input = Input_Controller.instance;
        
        input.OnSelect += OnPointer_Select;
        input.OnMultiSelect += OnPointer_MultiSelect;
    }

    private void OnDestroy()
    {
        Input_Controller input = Input_Controller.instance;
        
        input.OnSelect -= OnPointer_Select;
        input.OnMultiSelect -= OnPointer_MultiSelect;
    }


    // EventSystems
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnEnter?.Invoke();
        _pointerEntered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnExit?.Invoke();
        _pointerEntered = false;
    }

    public void OnPointer_Select()
    {
        if (!_pointerEntered) return;
        OnSelect?.Invoke();
    }

    public void OnPointer_MultiSelect()
    {
        if (!_pointerEntered) return;
        OnMultiSelect?.Invoke();
    }
}
