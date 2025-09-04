using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Clickable_EventSystem : MonoBehaviour, IPointerClickHandler
{
    public Action OnClick;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke();
    }
}
