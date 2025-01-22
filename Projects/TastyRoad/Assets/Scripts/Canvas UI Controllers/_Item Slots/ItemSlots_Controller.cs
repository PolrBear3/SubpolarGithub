using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlots_Controller : MonoBehaviour
{
    [Header("")]
    [SerializeField] private ItemSlot_Cursor _cursor;
    public ItemSlot_Cursor cursor => _cursor;

    [SerializeField] private List<ItemSlot> _itemSlots = new();
    public List<ItemSlot> itemSlots => _itemSlots;

    [Header("")]
    [SerializeField][Range(0, 1000)] private int _singleSlotCapacity;
    public int singleSlotCapacity => _singleSlotCapacity;

    [SerializeField][Range(0, 10)] private int _maxPageNum;
    public int maxPageNum => _maxPageNum;


    // UnityEngine
    private void Start()
    {
        Set_GridNums();
    }


    // Set Slot
    private void Set_GridNums()
    {
        Vector2 gridNumTrack = Vector2.zero;

        for (int i = 0; i < _itemSlots.Count; i++)
        {
            _itemSlots[i].Assign_GridNum(gridNumTrack);
            gridNumTrack.x++;
        }
    }


    public void Set_Datas(List<ItemSlot_Data> setDatas)
    {
        for (int i = 0; i < setDatas.Count; i++)
        {
            _itemSlots[i].Assign_Data(setDatas[i]);
        }
    }

    public void SlotsAssign_Update()
    {
        foreach (var slot in _itemSlots)
        {
            slot.Update_SlotIcon();
            slot.AmountText_Update();

            slot.Toggle_BookMark(slot.data.bookMarked);
            slot.Toggle_Lock(slot.data.isLocked);

            slot.Toggle_MaterialShine(false);
        }
    }


    // Current Slots
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


    public List<ItemSlot_Data> CurrentSlots_toDatas()
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


    // Data
    public void AddNewPage_ItemSlotDatas(Dictionary<int, List<ItemSlot_Data>> targetData)
    {
        if (targetData.Count >= _maxPageNum) return;

        // set new slot datas, amount of current _itemSlots
        List<ItemSlot_Data> newDatas = new();
        for (int i = 0; i < _itemSlots.Count; i++)
        {
            newDatas.Add(new());
        }

        // add new data to targetData
        targetData.Add(targetData.Count, newDatas);
    }


    public ItemSlot_Data SlotData(Dictionary<int, List<ItemSlot_Data>> datas, ItemSlot_Data data)
    {
        for (int i = 0; i < datas.Count; i++)
        {
            for (int j = 0; j < datas[i].Count; j++)
            {
                if (data != datas[i][j]) continue;
                return datas[i][j];
            }
        }
        return null;
    }

    public ItemSlot_Data Empty_SlotData(Dictionary<int, List<ItemSlot_Data>> datas)
    {
        for (int i = 0; i < datas.Count; i++)
        {
            for (int j = 0; j < datas[i].Count; j++)
            {
                if (datas[i][j].hasItem) continue;
                return datas[i][j];
            }
        }
        return null;
    }

    public List<ItemSlot_Data> BookMarked_Datas(Dictionary<int, List<ItemSlot_Data>> datas, bool isLocked)
    {
        List<ItemSlot_Data> bookmarkedDatas = new();

        for (int i = 0; i < datas.Count; i++)
        {
            for (int j = 0; j < datas[i].Count; j++)
            {
                if (datas[i][j].hasItem == false) continue;
                if (datas[i][j].bookMarked == false) continue;
                if (datas[i][j].isLocked != isLocked) continue;

                bookmarkedDatas.Add(datas[i][j]);
            }
        }

        return bookmarkedDatas;
    }


    // Station
    public List<ItemSlot_Data> Station_SlotDatas(Dictionary<int, List<ItemSlot_Data>> datas, Station_ScrObj targetStation)
    {
        List<ItemSlot_Data> targetDatas = new();

        for (int i = 0; i < datas.Count; i++)
        {
            for (int j = 0; j < datas[i].Count; j++)
            {
                if (datas[i][j].hasItem == false) continue;
                if (targetStation != datas[i][j].currentStation) continue;
                targetDatas.Add(datas[i][j]);

            }
        }
        return targetDatas;
    }
    public List<ItemSlot_Data> Station_SlotDatas(Dictionary<int, List<ItemSlot_Data>> datas, Station_ScrObj targetStation, bool bookMarked)
    {
        List<ItemSlot_Data> targetDatas = new();

        for (int i = 0; i < datas.Count; i++)
        {
            for (int j = 0; j < datas[i].Count; j++)
            {
                if (datas[i][j].hasItem == false) continue;
                if (datas[i][j].bookMarked != bookMarked) continue;
                if (targetStation != datas[i][j].currentStation) continue;
                targetDatas.Add(datas[i][j]);

            }
        }
        return targetDatas;
    }


    public int StationAmount(Dictionary<int, List<ItemSlot_Data>> datas, Station_ScrObj targetStation)
    {
        int stationCount = 0;

        for (int i = 0; i < datas.Count; i++)
        {
            for (int j = 0; j < datas[i].Count; j++)
            {
                if (datas[i][j].hasItem == false) continue;
                if (targetStation != datas[i][j].currentStation) continue;
                stationCount++;
            }
        }

        return stationCount;
    }

    public void Stations_ToggleLock(Dictionary<int, List<ItemSlot_Data>> datas, Station_ScrObj targetStation, bool isLock)
    {
        for (int i = 0; i < datas.Count; i++)
        {
            for (int j = 0; j < datas[i].Count; j++)
            {
                if (datas[i][j].hasItem == false) continue;
                if (targetStation != datas[i][j].currentStation) continue;

                datas[i][j].isLocked = isLock;
            }
        }
    }


    // Food
    public int FoodAmount(Dictionary<int, List<ItemSlot_Data>> datas, Food_ScrObj targetFood)
    {
        int foodCount = 0;

        for (int i = 0; i < datas.Count; i++)
        {
            for (int j = 0; j < datas[i].Count; j++)
            {
                if (datas[i][j].hasItem == false) continue;
                if (targetFood != datas[i][j].currentFood) continue;
                foodCount += datas[i][j].currentAmount;
            }
        }

        return foodCount;
    }

    public void Foods_ToggleLock(Dictionary<int, List<ItemSlot_Data>> datas, Food_ScrObj targetFood, bool isLock)
    {
        for (int i = 0; i < datas.Count; i++)
        {
            for (int j = 0; j < datas[i].Count; j++)
            {
                if (datas[i][j].hasItem == false) continue;
                if (targetFood != datas[i][j].currentFood) continue;

                datas[i][j].isLocked = isLock;
            }
        }
    }
}