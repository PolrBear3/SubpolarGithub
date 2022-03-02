using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable_Objects : MonoBehaviour
{
    public delegate void DragEndedDelegate(Draggable_Objects draggableObjects);

    public DragEndedDelegate dragEndedCallBack;
    
    private bool isDragged = false;
    private Vector3 mouseDragStartPosition;
    private Vector3 objectDragStartPosition;

    private void OnMouseDown()
    {
        isDragged = true;
        mouseDragStartPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        objectDragStartPosition = transform.localPosition;
    }

    private void OnMouseDrag()
    {
        if (isDragged)
        {
            transform.localPosition = objectDragStartPosition + (Camera.main.ScreenToWorldPoint(Input.mousePosition) - mouseDragStartPosition);
        }
    }

    private void OnMouseUp()
    {
        isDragged = false;
        dragEndedCallBack(this);
    }
}
