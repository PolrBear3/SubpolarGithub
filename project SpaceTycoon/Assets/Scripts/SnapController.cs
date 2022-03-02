using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapController : MonoBehaviour
{
    public List<Transform> snapPoints;
    public List<Draggable_Objects> draggableObjects;
    public float snapRange = 0.2f;

    void Start()
    {
        foreach(Draggable_Objects draggableObjects in draggableObjects)
        {
            draggableObjects.dragEndedCallBack = OnDragEnded;
        }
    }

    private void OnDragEnded(Draggable_Objects draggableObjects)
    {
        float closestDistance = -1;
        Transform closestSnapPoint = null;

        foreach(Transform snapPoint in snapPoints)
        {
            float currentDistance = Vector2.Distance(draggableObjects.transform.localPosition, snapPoint.localPosition);
            if (closestSnapPoint == null || currentDistance < closestDistance)
            {
                closestSnapPoint = snapPoint;
                closestDistance = currentDistance;
            }
        }

        if (closestSnapPoint != null && closestDistance <= snapRange)
        {
            draggableObjects.transform.localPosition = closestSnapPoint.localPosition;
        }
    }
}
