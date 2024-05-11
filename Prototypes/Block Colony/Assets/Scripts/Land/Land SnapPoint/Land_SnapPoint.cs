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
        if (MainController.actionAvailable == false) return;

        Cursor cursor = _main.cursor;

        // if card not dragging, return
        if (!cursor.isDragging) return;

        // get cursor gameobject > get ISnapPointInteractable
        if (!cursor.dragCardGameObject.TryGetComponent(out ISnapPointInteractable interactable))
        {
            cursor.dragCard.Return();
            cursor.Clear_Card();
            return;
        }

        // check if interact available
        if (cursor.dragCardGameObject.TryGetComponent(out IInteractCheck check))
        {
            if (check.InteractAvailable() == false)
            {
                cursor.dragCard.Return();
                cursor.Clear_Card();
                return;
            }
        }

        // update order layer
        if (cursor.dragCardGameObject.TryGetComponent(out SpriteRenderer sr)) sr.sortingOrder -= 2;

        interactable.Interact();

        // cursor
        cursor.dragCard.Use();
        cursor.Clear_Card();
    }

    public void OnPointerEnter()
    {
        _main.cursor.Update_HoverObject(gameObject);

        // tooltip
        if (_main.cursor.isDragging) return;
        if (_currentData.hasLand == false) return;

        _main.toolTip.Toggle(true);
        _main.toolTip.DescriptionText_Update(_currentData.currentLand.CurrentEvents_Description());
    }

    public void OnPointerExit()
    {
        _main.cursor.Update_HoverObject(null);

        // tooltip
        _main.toolTip.Toggle(false);
    }


    // Set Functions
    public void Set_CurrentData(SnapPointData setData)
    {
        _currentData = setData;
    }
}
