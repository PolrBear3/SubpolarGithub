using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISnapPointInteractable
{
    void Interact();
}

public class Land_SnapPoint : MonoBehaviour
{
    private MainController _main;

    private SnapPointData _currentData;
    public SnapPointData currentData => _currentData;


    // UnityEngine
    private void Awake()
    {
        _main = GameObject.FindGameObjectWithTag("MainController").GetComponent<MainController>();
    }


    // EventTrigger
    public void OnPointerClick()
    {
        Cursor cursor = _main.cursor;

        // if card not dragging, return
        if (!cursor.isDragging) return;

        // get cursor gameobject > get ISnapPointInteractable
        if (!cursor.dragCardGameObject.TryGetComponent(out ISnapPointInteractable interactable)) return;

        // check if interact available
        if (cursor.dragCardGameObject.TryGetComponent(out IInteractCheck check))
        {
            if (check.InteractAvailable() == false) return;
        }

        interactable.Interact();

        // cursor
        cursor.dragCard.Use();
        cursor.Clear_Card();
    }

    public void OnPointerEnter()
    {
        _main.cursor.Update_HoverObject(gameObject);
    }

    public void OnPointerExit()
    {
        _main.cursor.Update_HoverObject(null);
    }


    // Set Functions
    public void Set_CurrentData(SnapPointData setData)
    {
        _currentData = setData;
    }
}
