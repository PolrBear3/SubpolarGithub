using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Controller : MonoBehaviour
{
    [HideInInspector] public Data_Controller dataController;

    private List<GameObject> _currentCharacters = new();
    private List<GameObject> _currentStations = new();

    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Data_Controller dataController)) { this.dataController = dataController; }
    }

    // Track
    public void Track_Character(GameObject character)
    {
        _currentCharacters.Add(character);
    }
    public void Track_Station(GameObject station)
    {
        _currentStations.Add(station);
    }

    // Spawn

}