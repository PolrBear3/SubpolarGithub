using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubLocations_Controller : MonoBehaviour
{
    [Header("")]
    [SerializeField] private Main_Controller _main;

    [Header("")]
    private List<SubLocation> _currentSubLocations = new();


    // Current Sub Locations Data Control
    public void Track(SubLocation subLocation)
    {
        _currentSubLocations.Add(subLocation);
        subLocation.transform.SetParent(transform);
    }

    public void ClearAll()
    {
        _currentSubLocations.Clear();
    }


    // Positioning on Game Scene
    public void RePosition()
    {
        int xPositionCount = 0;

        for (int i = 0; i < _currentSubLocations.Count; i++)
        {
            _currentSubLocations[i].transform.position = new Vector2(xPositionCount, 11.25f);
            xPositionCount += 20;
        }
    }
}
