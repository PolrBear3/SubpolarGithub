using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootBox_Data
{
    [ES3Serializable] private bool _dropped;
    public bool dropped => _dropped;

    [ES3Serializable] private HashSet<WorldMap_Data> _droppedMapHistory = new();
    public HashSet<WorldMap_Data> droppedMapHistory => _droppedMapHistory;


    // Constructors
    public LootBox_Data(bool dropped)
    {
        _dropped = dropped;
    }


    // Functions
    public void Toggle_DropStatus(bool toggle)
    {
        _dropped = toggle;
    }

    public void Add_MapHistory(WorldMap_Data data)
    {
        _droppedMapHistory.Add(data);
    }
}
