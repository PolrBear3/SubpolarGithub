using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlots_Controller : MonoBehaviour
{
    [Header("")]
    [SerializeField] private Transform _itemSlotsLayout;

    private List<ItemSlot> _itemSlots = new();
    public List<ItemSlot> itemSlots => _itemSlots;

    [Header("")]
    [SerializeField] private int _maxGridNumX;

    [SerializeField] private int _singleSlotCapacity;
    public int singleSlotCapacity => _singleSlotCapacity;

    [Header("")]
    [SerializeField] private GameObject _itemSlotPrefab;


    //
    public void Add_Slot(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            // instantiate item slots prefab in All Item Slots
            GameObject addSlot = Instantiate(_itemSlotPrefab, _itemSlotsLayout);
            ItemSlot newSlot = addSlot.GetComponent<ItemSlot>();

            addSlot.transform.SetParent(_itemSlotsLayout);

            // add instantiated item slots to _itemSlots
            _itemSlots.Add(newSlot);

            // set data
            newSlot.Assign_Data(new());
        }

        Set_Slots_GridNum();
    }

    private void Set_Slots_GridNum()
    {
        Vector2 gridCount = Vector2.zero;

        for (int i = 0; i < _itemSlots.Count; i++)
        {
            _itemSlots[i].Assign_GridNum(gridCount);

            gridCount.x++;

            if (gridCount.x != _maxGridNumX + 1) continue;

            gridCount.x = 0;
            gridCount.y++;
        }
    }


    //
    public bool Slots_Full()
    {
        for (int i = 0; i < _itemSlots.Count; i++)
        {
            if (_itemSlots[i].data.hasItem) continue;
            return false;
        }

        return true;
    }


    public List<ItemSlot> BookMarked_Slots()
    {
        List<ItemSlot> bookmarkedSlots = new();

        for (int i = 0; i < _itemSlots.Count; i++)
        {
            if (_itemSlots[i].data.hasItem == false) continue;
            if (_itemSlots[i].data.bookMarked == false) continue;

            bookmarkedSlots.Add(_itemSlots[i]);
        }

        return bookmarkedSlots;
    }

    public List<ItemSlot> LockedSlots()
    {
        List<ItemSlot> lockedSlots = new();

        for (int i = 0; i < _itemSlots.Count; i++)
        {
            if (_itemSlots[i].data.hasItem == false) continue;
            if (_itemSlots[i].data.isLocked == false) continue;

            lockedSlots.Add(_itemSlots[i]);
        }

        return lockedSlots;
    }
}
