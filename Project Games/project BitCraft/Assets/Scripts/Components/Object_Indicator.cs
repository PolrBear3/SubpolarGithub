using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Object_Indicator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Prefab_Controller _prefabController;

    private void Awake()
    {
        if (gameObject.TryGetComponent(out Prefab_Controller prefabController)) { _prefabController = prefabController; }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Object_Scanner scanner = _prefabController.tilemapController.controller.inventoryController.dragSlot.objectScanner;

        if (_prefabController.prefabTag.prefabType == Prefab_Type.overlapPlaceable)
        {
            scanner.Show_Amount(_prefabController);
        }
        else if (_prefabController.prefabTag.prefabType == Prefab_Type.placeable)
        {
            if (_prefabController.statController == null) return;
            scanner.Show_Life(_prefabController);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        Object_Scanner scanner = _prefabController.tilemapController.controller.inventoryController.dragSlot.objectScanner;
        scanner.Hide_Amount();
        scanner.Hide_Life();
    }
}
