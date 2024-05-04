using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SnapPointData
{
    [SerializeField] private Vector2 _gridNum;
    public Vector2 gridNum => _gridNum;

    [SerializeField] private Land _currentLand;
    public Land currentLand => _currentLand;

    private bool _hasLand;
    public bool hasLand => _hasLand;


    // Constructors
    public SnapPointData(Vector2 gridNum)
    {
        _gridNum = gridNum;
    }


    // Functions
    public void Update_CurrentLand(Land updateLand)
    {
        if (updateLand == null)
        {
            _currentLand = null;
            _hasLand = false;

            return;
        }

        _currentLand = updateLand;
        _hasLand = true;
    }
}
