using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_Controller : MonoBehaviour
{
    [SerializeField] private List<Slot> _slots = new List<Slot>();
    public List<Slot> slots { get => _slots; set => _slots = value; }

    private void Awake()
    {
        Set_Slots();
    }

    private void Set_Slots()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].Set_Inventory_Controller(this);
        }
    }
}