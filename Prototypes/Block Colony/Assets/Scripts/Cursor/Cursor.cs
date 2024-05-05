using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


    // Hover Update
    public void Update_HoverObject(GameObject hoverObject)
    {
        _hoveringObject = hoverObject;
    }


    // Drag
    public void Drag_Card(Card dragCard)
    {
        GameObject spawnedPrefab = Instantiate(dragCard.currentData.scrObj.cardPrefab, transform.position, Quaternion.identity);

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
