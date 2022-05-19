using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Static_Box_Display : Box_Display
{
    [SerializeField] private Box_Holder _boxHolder;
    [SerializeField] private BoxSlot_UI[] _slots;
    public BoxSlot_UI[] slots => _slots;

    protected override void Start()
    {
        base.Start();

        if (_boxHolder != null)
        {
            _boxSystem = _boxHolder.boxSystem;
            _boxSystem.boxSlotUpdate += Update_Slot;
        }

        Assign_Slot(_boxSystem);
    }

    public override void Assign_Slot(Box_System boxToDisplay)
    {
        _slotDictionary = new Dictionary<BoxSlot_UI, Box_Slot>();

        for (int i = 0; i < _boxSystem.boxSize; i++)
        {
            _slotDictionary.Add(_slots[i], _boxSystem.boxSlots[i]);
            _slots[i].Init(_boxSystem.boxSlots[i]);
        }
    }
}