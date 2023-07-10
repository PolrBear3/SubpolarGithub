using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment_Controller : MonoBehaviour
{
    [SerializeField] private Prefab_Controller _prefabController;

    [SerializeField] private List<Prefab_Controller> _currentEquipments;
    public List<Prefab_Controller> currentEquipments { get => _currentEquipments; set => _currentEquipments = value; }

    //
    private void Awake()
    {
        if (transform.parent.TryGetComponent(out Prefab_Controller prefabController)) { _prefabController = prefabController; }
    }

    // Update Functions
    public void Update_Current_Equipments()
    {
        _currentEquipments.Clear();
        // remove all child gameobjects ??

        Prefabs_Data data = _prefabController.tilemapController.controller.prefabsData;

        List<Slot> currentSlots = _prefabController.tilemapController.controller.inventoryController.slots;

        for (int i = 0; i < currentSlots.Count; i++)
        {
            if (!currentSlots[i].hasItem) continue;

            Prefab_Controller itemObject = data.Get_Object_PrefabController(currentSlots[i].currentItem.id);

            if (!itemObject.TryGetComponent(out IEquippable equippable)) continue;

            Instantiate(itemObject.prefabTag.Prefab(), transform);
            _currentEquipments.Add(itemObject);
            // make equipment transparent ??
        }
    }
}
