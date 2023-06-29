using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_Controller : MonoBehaviour
{
    [SerializeField] private Game_Controller _controller;
    public Game_Controller controller { get => _controller; set => _controller = value; }

    [SerializeField] private Drag_Slot _dragSlot;
    public Drag_Slot dragSlot { get => _dragSlot; set => _dragSlot = value; }

    private List<Slot> _slots = new List<Slot>();
    public List<Slot> slots { get => _slots; set => _slots = value; }

    private void Awake()
    {
        Set_Slots();
    }
    private void Start()
    {
        
    }

    // Check
    public bool Is_Inventory_Full()
    {
        for (int i = 0; i < _slots.Count; i++)
        {
            if (!_slots[i].hasItem) return false;
        }
        return true;
    }

    // Get
    public Slot Empty_Slot()
    {
        for (int i = 0; i < _slots.Count; i++)
        {
            if (_slots[i].hasItem) continue;
            return _slots[i];
        }
        return null;
    }

    // Setup
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

    // Function
    public void Add_Item(Item_ScrObj item, int amount)
    {
        if (amount <= 0) return;
        
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
    public void Add_Item(int itemID, int amount)
    {
        if (amount <= 0) return;

        Item_ScrObj item = controller.prefabsData.Get_Item(itemID);

        if (item == null)
        {
            Debug.Log("Invalid Item ID");
            return;
        }

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