using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_Controller : MonoBehaviour
{
    private List<Slot> _slots = new List<Slot>();
    public List<Slot> slots { get => _slots; set => _slots = value; }

    private void Awake()
    {
        Set_Slots();
    }

    private void Set_Slots()
    {
        int slotCount = transform.childCount;
        
        for (int i = 0; i < slotCount; i++)
        {
            GameObject slotGameObject = transform.GetChild(i).gameObject;
            
            if (slotGameObject.TryGetComponent(out Slot slot)) { _slots.Add(slot); }

            _slots[i].Set_Inventory_Controller(this);
            _slots[i].Clear();
        }
    }
}