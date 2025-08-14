using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MainController_Data
{
    [ES3Serializable] private List<Vector2> _claimedPositions = new();
    public List<Vector2> claimedPositions => _claimedPositions;

    [ES3Serializable] private List<Station_LoadData> _stationLoadDatas = new();
    public List<Station_LoadData> stationLoadDatas => _stationLoadDatas;
    
    [ES3Serializable] private List<Food_ScrObj> _bookmarkedFoods = new();
    public List<Food_ScrObj> bookmarkedFoods => _bookmarkedFoods;
    
    
    // Claimed Positions
    public bool Position_Claimed(Vector2 checkPosition)
    {
        return _claimedPositions.Contains(checkPosition);
    }
    
    public void Claim_Position(Vector2 position)
    {
        if (Position_Claimed(position)) return;
        _claimedPositions.Add(position);
    }
    
    
    // Food Bookmarking
    public void Remove_DuplicateBookmarks()
    {
        HashSet<Food_ScrObj> hashFoods = new();
        List<Food_ScrObj> nonDuplicateFoods = new();

        for (int i = 0; i < _bookmarkedFoods.Count; i++)
        {
            // if _bookmarkedFoods[i] is not in hashFoods, add & return true
            if (!hashFoods.Add(_bookmarkedFoods[i])) continue;

            // also add to nonDuplicateFoods
            nonDuplicateFoods.Add(_bookmarkedFoods[i]);
        }

        _bookmarkedFoods = nonDuplicateFoods;
    }
}
