using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlots_Controller : MonoBehaviour
{
    [Header("")]
    [SerializeField] private ItemSlot_Cursor _cursor;
    public ItemSlot_Cursor cursor => _cursor;

    [Header("")]
    [SerializeField] private List<ItemSlot> _itemSlots = new();
    public List<ItemSlot> itemSlots => _itemSlots;

    [SerializeField] [Range(0, 1000)] private int _singleSlotCapacity;
    public int singleSlotCapacity => _singleSlotCapacity;


    // UnityEngine
    private void Start()
    {
        Set_GridNums();
    }


    // Set
    private void Set_GridNums()
    {
        Vector2 gridNumTrack = Vector2.zero;
        float recentPosY = _itemSlots[0].rectTransform.position.y;

        for (int i = 0; i < _itemSlots.Count; i++)
        {
            if (_itemSlots[i].rectTransform.position.y != recentPosY)
            {
                gridNumTrack.x = 0;
                gridNumTrack.y++;
            }

            _itemSlots[i].Assign_GridNum(gridNumTrack);

            recentPosY = _itemSlots[i].rectTransform.position.y;
            gridNumTrack.x++;
        }
    }


    public void Set_Datas(List<ItemSlot_Data> setDatas)
    {
        for (int i = 0; i < setDatas.Count; i++)
        {
            if (i >= _itemSlots.Count) return;
            _itemSlots[i].Assign_Data(setDatas[i]);
        }
    }

    public void SlotsAssign_Update()
    {
        foreach (var slot in _itemSlots)
        {
            slot.Assign_Item();
            slot.Assign_Amount(slot.data.currentAmount);

            slot.Toggle_BookMark(slot.data.bookMarked);
            slot.Toggle_Lock(slot.data.isLocked);
        }
    }


    // Checks and Gets
    public ItemSlot ItemSlot(Vector2 gridNum)
    {
        for (int i = 0; i < _itemSlots.Count; i++)
        {
            if (gridNum != _itemSlots[i].gridNum) continue;
            return _itemSlots[i];
        }
        return null;
    }

    public ItemSlot EmptySlot()
    {
        for (int i = 0; i < _itemSlots.Count; i++)
        {
            if (_itemSlots[i].data.hasItem == true) continue;
            return _itemSlots[i];
        }
        return null;
    }


    public bool Slots_Full()
    {
        for (int i = 0; i < _itemSlots.Count; i++)
        {
            if (_itemSlots[i].data.hasItem) continue;
            return false;
        }

        return true;
    }

    public int AvailableSlots_Count()
    {
        int slotsCount = 0;

        foreach (var slot in _itemSlots)
        {
            if (slot.data.hasItem == true) continue;
            slotsCount++;
        }

        return slotsCount;
    }


    /// <returns>
    /// All datas from _itemSlots
    /// </returns>
    public List<ItemSlot_Data> Current_SlotDatas()
    {
        List<ItemSlot_Data> currentDatas = new();

        foreach (var slot in _itemSlots)
        {
            currentDatas.Add(slot.data);
        }

        return currentDatas;
    }


    public List<ItemSlot> BookMarked_Slots(bool isLocked)
    {
        List<ItemSlot> bookmarkedSlots = new();

        for (int i = 0; i < _itemSlots.Count; i++)
        {
            if (_itemSlots[i].data.hasItem == false) continue;
            if (_itemSlots[i].data.bookMarked == false) continue;
            if (_itemSlots[i].data.isLocked != isLocked) continue;

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
