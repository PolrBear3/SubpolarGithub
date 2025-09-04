using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VehicleMenu_Data
{
    [ES3Serializable] private Dictionary<int, List<ItemSlot_Data>> _slotDatas = new();
    public Dictionary<int, List<ItemSlot_Data>> slotDatas => _slotDatas;
}
