using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Controller : MonoBehaviour
{
    [HideInInspector] public Data_Controller dataController;
    [HideInInspector] public Spawn_Controller spawnController;

    [HideInInspector] public Location_Controller currentLocation;
    [HideInInspector] public List<GameObject> currentCharacters = new();
    [HideInInspector] public List<GameObject> currentStations = new();

    [Header("Opening Panel")]
    [SerializeField] private float _openingTime;
    [SerializeField] private GameObject _openingPanel;

    [Header("Data")]
    public int currentCoin;

    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Data_Controller dataController)) { this.dataController = dataController; }
        if (gameObject.TryGetComponent(out Spawn_Controller spawnController)) { this.spawnController = spawnController; }
    }
    private void Start()
    {
        LoadGame_Animation();
    }

    // Track Current Prefabs
    public void Connect_Location(Location_Controller location)
    {
        currentLocation = location;
    }
    public void Connect_Character(GameObject character)
    {
        currentCharacters.Add(character);
    }
    public void Connect_Station(GameObject station)
    {
        currentStations.Add(station);
    }

    //
    private void LoadGame_Animation()
    {
        _openingPanel.SetActive(true);
        LeanTween.moveLocalX(_openingPanel, -2000f, _openingTime);
    }
}