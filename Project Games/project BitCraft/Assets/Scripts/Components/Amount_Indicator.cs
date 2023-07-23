using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Amount_Indicator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Prefab_Controller _prefabController;

    private void Awake()
    {
        if (gameObject.TryGetComponent(out Prefab_Controller prefabController)) { _prefabController = prefabController; }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Object_Scanner scanner = _prefabController.tilemapController.controller.inventoryController.dragSlot.objectScanner;
        scanner.Show_Amount(_prefabController.currentAmount);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        Object_Scanner scanner = _prefabController.tilemapController.controller.inventoryController.dragSlot.objectScanner;
        scanner.Hide_Amount();
    }
}
