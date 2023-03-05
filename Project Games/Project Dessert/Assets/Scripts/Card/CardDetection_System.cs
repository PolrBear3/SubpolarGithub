using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDetection_System : MonoBehaviour
{
    [SerializeField] private GameObject detectionBox;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out DragDrop_System system)) return;
        if (!system.attached) return;

        LeanTween.alpha(detectionBox, 0.7f, 0.25f);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        LeanTween.alpha(detectionBox, 0, 0.25f);
    }
}