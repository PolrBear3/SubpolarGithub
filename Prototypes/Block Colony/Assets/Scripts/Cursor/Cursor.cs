using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Cursor : MonoBehaviour
{
    private Card _dragCard;
    public Card dragCard => _dragCard;

    private GameObject _dragCardGameObject;
    public GameObject dragCardGameObject => _dragCardGameObject;

    private GameObject _hoveringObject;
    public GameObject hoveringObject => _hoveringObject;

    private bool _isDragging;
    public bool isDragging => _isDragging;

    [SerializeField] private Transform _dragPoint;


    // MonoBehaviour
    private void Update()
    {
        if (isDragging == false) return;

        CursorFollow_Update();
    }


    // InputSystem
    private void OnLeftSelect()
    {
        if (_isDragging == false || _hoveringObject != null) return;

        _dragCard.Return();
        Clear_Card();
    }
    private void OnRightSelect()
    {
        if (_isDragging == false) return;

        _dragCard.Return();
        Clear_Card();
    }


    // Cursor Position Update
    private void CursorFollow_Update()
    {
        Vector2 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        transform.position = mousePos;
    }


    // Hover Update
    public void Update_HoverObject(GameObject hoverObject)
    {
        _hoveringObject = hoverObject;
    }


    // Drag
    public void Drag_Card(Card dragCard)
    {
        GameObject spawnedPrefab = Instantiate(dragCard.currentData.scrObj.cardPrefab, _dragPoint.position, Quaternion.identity);
        spawnedPrefab.transform.SetParent(_dragPoint);

        // update order layer
        if (spawnedPrefab.TryGetComponent(out SpriteRenderer sr)) sr.sortingOrder++;

        _dragCard = dragCard;
        _dragCardGameObject = spawnedPrefab;

        _isDragging = true;
    }

    // Drop
    public void Drop_Card()
    {
        _dragCard = null;
        _dragCardGameObject = null;

        _isDragging = false;
    }

    // Clear
    public void Clear_Card()
    {
        Destroy(_dragCardGameObject);
        Drop_Card();
    }
}
