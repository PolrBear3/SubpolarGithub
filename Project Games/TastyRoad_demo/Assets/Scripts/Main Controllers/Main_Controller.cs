using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main_Controller : MonoBehaviour
{
    [HideInInspector] public Data_Controller dataController;

    [SerializeField] private Camera _mainCamera;

    [Header("File Locations")]
    [SerializeField] private Transform _locationFile;
    [SerializeField] private Transform _characterFile;
    [SerializeField] private Transform _stationFile;

    [Header("Current Data")]
    public Location_Controller currentLocation;
    public List<GameObject> currentCharacters = new();
    public List<GameObject> currentStations = new();

    public int currentCoin;

    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Data_Controller dataController)) { this.dataController = dataController; }
    }

    // Camera
    public Vector2 OuterCamera_Position(float horizontal, float vertical, float positionX, float positionY)
    {
        float horizontalPos = horizontal;
        float verticalPos = vertical;

        if (horizontalPos == 0) horizontalPos = 0.5f;
        else if (horizontalPos == -1f) horizontalPos = 0f;

        if (verticalPos == 0) verticalPos = 0.5f;
        else if (verticalPos == -1f) verticalPos = 0f;

        Vector2 cameraPosition = _mainCamera.ViewportToWorldPoint(new Vector2(horizontalPos, verticalPos));

        return new Vector2(cameraPosition.x + positionX, cameraPosition.y + positionY);
    }

    // Current Location Control
    public void Track_CurrentLocaiton(Location_Controller location)
    {
        currentLocation = location;
    }

    public void UnTrack_CurrentLocation(Location_Controller location)
    {
        currentLocation = null;
    }

    public void Set_Location(int arrayNum)
    {
        GameObject location = Instantiate(dataController.locations[arrayNum], Vector2.zero, Quaternion.identity);
        location.transform.parent = _locationFile;
    }

    // Current Characters Control
    public void Track_CurrentCharacter(GameObject character)
    {
        currentCharacters.Add(character);
    }

    public void UnTrack_CurrentCharacter(GameObject character)
    {
        currentCharacters.Remove(character);
    }

    public void Spawn_Character(int arrayNum, Vector2 spawnPosition)
    {
        GameObject character = Instantiate(dataController.characters[arrayNum], spawnPosition, Quaternion.identity);
        character.transform.parent = _characterFile;
    }

    // Current Stations Control
    public void Track_CurrentStation(GameObject station)
    {
        currentStations.Add(station);
    }

    public void UnTrack_CurrentStation(GameObject station)
    {
        currentStations.Remove(station);
    }

    public void Spawn_Station(int arrayNum, Vector2 spawnPosition)
    {
        GameObject station = Instantiate(dataController.stations[arrayNum], spawnPosition, Quaternion.identity);
        station.transform.parent = _stationFile;
    }
}