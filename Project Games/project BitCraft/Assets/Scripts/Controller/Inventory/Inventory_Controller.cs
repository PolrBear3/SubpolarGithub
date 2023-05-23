using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_Controller : MonoBehaviour
{
    [SerializeField] private Drag_Slot _dragSlot;
    public Drag_Slot dragSlot { get => _dragSlot; set => _dragSlot = value; }

    private List<Slot> _slots = new List<Slot>();
    public List<Slot> slots { get => _slots; set => _slots = value; }

    public Item_ScrObj log;
    public Item_ScrObj water;

    private void Awake()
    {
        Set_Slots();
        _slots[0].Assign(log, 4);
        _slots[1].Assign(log, 4);
        _slots[2].Assign(water, 4);
        _slots[3].Assign(log, 48);
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

    // functions
    public void Add_Item(Item_ScrObj item, int amount)
    {
        for (int i = 0; i < _slots.Count; i++)
        {
            if (_slots[i].currentAmount <= 0)
            {
                _slots[i].Assign(item, amount);
                break;
            }

            if (_slots[i].Is_Max_Amount()) continue;
            if (!_slots[i].Is_Same_Item(item)) continue;

            _slots[i].Increase_Amount(amount);
            break;
        }
    }
}