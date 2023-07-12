using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment_Controller : MonoBehaviour
{
    private Prefab_Controller _prefabController;

    [SerializeField] private List<Prefab_Controller> _inventoryEquipments;
    public List<Prefab_Controller> inventoryEquipments { get => _inventoryEquipments; set => _inventoryEquipments = value; }

    [SerializeField] private Prefab_Controller _currentEquipment;
    public Prefab_Controller currentEquipment { get => _currentEquipment; set => _currentEquipment = value; }

    //
    private void Awake()
    {
        if (transform.parent.TryGetComponent(out Prefab_Controller prefabController)) { _prefabController = prefabController; }
    }

    // Update Functions
    public void Update_Current_Equipments()
    {
        // reset
        _currentEquipment = null;

        _inventoryEquipments.Clear();

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // data
        Prefabs_Data data = _prefabController.tilemapController.controller.prefabsData;
        Inventory_Controller inventoryController = _prefabController.tilemapController.controller.inventoryController;

        List<Slot> currentSlots = _prefabController.tilemapController.controller.inventoryController.slots;

        // add current equipments from inventory
        for (int i = 0; i < currentSlots.Count; i++)
        {
            if (!currentSlots[i].hasItem) continue;

            // check duplicate equipment prefab controllers
            bool hasDuplicate = false;
            for (int j = 0; j < _inventoryEquipments.Count; j++)
            {
                if (_inventoryEquipments[j].prefabTag.prefabID != currentSlots[i].currentItem.id) continue;
                hasDuplicate = true;
                break;
            }
            if (hasDuplicate) continue;

            Prefab_Controller searchObject = data.Get_Object_PrefabController(currentSlots[i].currentItem.id);
            if (!searchObject.TryGetComponent(out IEquippable equippable)) continue;

            GameObject prefabObject = Instantiate(searchObject.prefabTag.Prefab(), transform);
            if (!prefabObject.TryGetComponent(out Prefab_Controller currentController)) continue;

            currentController.Connect_Components(_prefabController.tilemapController);

            // current equipment update
            if (inventoryController.equippedSlot.hasItem && inventoryController.equippedSlot.currentItem.id == currentController.prefabTag.prefabID)
            {
                _currentEquipment = currentController;
            }

            _inventoryEquipments.Add(currentController);
            currentController.sr.color = Color.clear;
        }
    }
}