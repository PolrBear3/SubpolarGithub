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

    [SerializeField][Range(0, 1000)] private int _singleSlotCapacity;
    public int singleSlotCapacity => _singleSlotCapacity;


    // UnityEngine
    private void Start()
    {
        Set_GridNums();
    }


    // Set Slot
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


    // Current Slot
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

    public List<ItemSlot_Data> BookMarked_Datas(Dictionary<int, List<ItemSlot_Data>> datas)
    {
        List<ItemSlot_Data> bookmarkedDatas = new();

        for (int i = 0; i < datas.Count; i++)
        {
            for (int j = 0; j < datas[i].Count; j++)
            {
                if (datas[i][j].hasItem == false) continue;
                if (datas[i][j].bookMarked == false) continue;

                bookmarkedDatas.Add(datas[i][j]);
            }
        }

        return bookmarkedDatas;
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


    public int FoodAmount(Dictionary<int, List<ItemSlot_Data>> datas, Food_ScrObj targetFood)
    {
        int foodCount = 0;

        for (int i = 0; i < datas.Count; i++)
        {
            for (int j = 0; j < datas[i].Count; j++)
            {
                if (datas[i][j].hasItem == false) continue;
                if (targetFood != datas[i][j].currentStation) continue;
                foodCount++;
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