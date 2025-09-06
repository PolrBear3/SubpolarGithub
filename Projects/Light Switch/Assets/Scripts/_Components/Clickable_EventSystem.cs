using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Clickable_EventSystem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Action OnEnter;
    public Action OnExit;
    public Action OnClick;
    
    
    // EventSystems
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnEnter?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnExit?.Invoke();
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke();
    }
}
