using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main_Controller : MonoBehaviour
{
    [HideInInspector] public Data_Controller dataController;

    [SerializeField] private Transform _characters;
    private List<GameObject> _currentCharacters = new();

    [SerializeField] private Transform _stations;
    private List<GameObject> _currentStations = new();

    private void Awake()
    {
        if (gameObject.TryGetComponent(out Data_Controller dataController)) { this.dataController = dataController; }
    }

    // Current Characters Control
    public void Track_CurrentCharacter(GameObject character)
    {
        _currentCharacters.Add(character);
    }

    public void Remove_CurrentCharacter(GameObject character)
    {
        _currentCharacters.Remove(character);
    }

    // Current Stations Control
    public void Track_CurrentStation(GameObject station)
    {
        _currentStations.Add(station);
    }

    public void Remove_CurrentStation(GameObject station)
    {
        _currentStations.Remove(station);
    }
}