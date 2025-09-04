using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MenuButton_Controller : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Space(20)]
    [SerializeField] private UnityEvent On_PointerEnter;
    [SerializeField] private UnityEvent On_PointerExit;
    [SerializeField] private UnityEvent On_PointerClick;
    
    
    // EventSystems
    public void OnPointerEnter(PointerEventData eventData)
    {
        On_PointerEnter?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        On_PointerExit?.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        On_PointerClick?.Invoke();
    }
}