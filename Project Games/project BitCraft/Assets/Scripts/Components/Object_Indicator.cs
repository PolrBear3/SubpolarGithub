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

        scanner.objectDetected = true;
        StartCoroutine(Show_Info());
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        Object_Scanner scanner = _prefabController.tilemapController.controller.inventoryController.dragSlot.objectScanner;

        scanner.Hide_Amount();
        scanner.Hide_Life();
    }

    IEnumerator Show_Info()
    {
        Object_Scanner scanner = _prefabController.tilemapController.controller.inventoryController.dragSlot.objectScanner;

        float elapsedTime = 0f;
        float hoverTime = scanner.hoverTime;

        while (scanner.objectDetected && elapsedTime < hoverTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (scanner.objectDetected && elapsedTime >= hoverTime)
        {
            if (_prefabController.prefabTag.prefabType == Prefab_Type.overlapPlaceable)
            {
                scanner.Show_Amount(_prefabController);
            }
            else if (_prefabController.prefabTag.prefabType == Prefab_Type.placeable)
            {
                if (_prefabController.statController != null)
                {
                    scanner.Show_Life(_prefabController);
                }
            }
        }
    }
}
