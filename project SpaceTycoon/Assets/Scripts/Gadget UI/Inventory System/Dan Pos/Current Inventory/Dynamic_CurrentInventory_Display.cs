using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Dynamic_CurrentInventory_Display : Box_Display
{
    [SerializeField] protected BoxSlot_UI slotPrefab;

    [SerializeField] private BoxSlot_UI[] _currentInventorySlots;
    public BoxSlot_UI[] currentInventorySlots => _currentInventorySlots;

    protected override void Start()
    {
        base.Start();
        Unlock_Current_Inventory_atStart();
    }

    public void Refresh_Dynamic_Box(Box_System boxToDisplay)
    {
        Clear_Slot();
        _boxSystem = boxToDisplay;
        Assign_Slot(boxToDisplay);
    }

    public override void Assign_Slot(Box_System boxToDisplay)
    {
        Clear_Slot();

        _slotDictionary = new Dictionary<BoxSlot_UI, Box_Slot>();

        if (boxToDisplay == null)
        {
            return;
        }

        for (int i = 0; i < boxToDisplay.boxSize; i++)
        {
            var uiSlot = Instantiate(slotPrefab, transform);
            _slotDictionary.Add(uiSlot, boxToDisplay.boxSlots[i]);
            uiSlot.Init(boxToDisplay.boxSlots[i]);
            uiSlot.Update_UISlot();
        }
    }

    private void Clear_Slot()
    {
        foreach (var item in transform.Cast<Transform>())
        {
            Destroy(item.gameObject);
        }

        if (_slotDictionary != null)
        {
            slotDictionary.Clear();
        }
    }

    void Unlock_Current_Inventory_atStart()
    {
        for (int i = 0; i < _currentInventorySlots.Length; i++)
        {
            _currentInventorySlots[i].assignedBoxSlot.unlocked = true;
        }
    }
}
