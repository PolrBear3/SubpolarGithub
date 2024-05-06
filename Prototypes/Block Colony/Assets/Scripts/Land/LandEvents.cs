using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandEvents : MonoBehaviour
{
    [SerializeField] private Land _land;
    public Land land => _land;

    [Header("")]
    [SerializeField] private GameObject[] _allEvents;


    // Functions
    public void Activate_AllEvents()
    {
        for (int i = 0; i < _allEvents.Length; i++)
        {
            if (!_allEvents[i].TryGetComponent(out ILandEventable landEvent)) continue;
            landEvent.Activate();
        }
    }
}
