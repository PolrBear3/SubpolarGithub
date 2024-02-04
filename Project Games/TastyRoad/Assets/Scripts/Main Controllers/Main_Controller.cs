using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main_Controller : MonoBehaviour
{
    [HideInInspector] public Data_Controller dataController;

    [SerializeField] private Camera _mainCamera;

    [SerializeField] private RectTransform _openingCurtain;

    [Header("File Locations")]
    [SerializeField] private Transform _locationFile;
    [SerializeField] private Transform _characterFile;
    [SerializeField] private Transform _stationFile;

    [HideInInspector] public int currentCoin;

    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Data_Controller dataController)) { this.dataController = dataController; }
    }

    private void Start()
    {
        LeanTween.alpha(_openingCurtain, 0f, 1f);
    }

    //
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



    // public Statics
    public static void Change_SpriteAlpha(SpriteRenderer sr, float alpha)
    {
        Color color = sr.color;
        color.a = alpha;
        sr.color = color;
    }



    // Position Control
    private List<Vector2> _claimedPositions = new();

    public void Claim_Position(Vector2 position)
    {
        _claimedPositions.Add(position);
    }
    public void UnClaim_Position(Vector2 position)
    {
        _claimedPositions.Remove(position);
    }

    public bool Position_Claimed(Vector2 checkPosition)
    {
        for (int i = 0; i < _claimedPositions.Count; i++)
        {
            if (checkPosition == _claimedPositions[i]) return true; 
        }
        return false;
    }



    // Current Location Control
    private Location_Controller _currentLocation;
    public Location_Controller currentLocation => _currentLocation;

    public void Track_CurrentLocaiton(Location_Controller location)
    {
        _currentLocation = location;
    }
    public void UnTrack_CurrentLocation(Location_Controller location)
    {
        _currentLocation = null;
    }
    public void Set_Location(int arrayNum)
    {
        GameObject location = Instantiate(dataController.locations[arrayNum], Vector2.zero, Quaternion.identity);
        location.transform.parent = _locationFile;
    }



    // Current Vechicle Control
    private Vehicle_Controller _currentVehicle;
    public Vehicle_Controller currentVehicle => _currentVehicle;

    public void Track_CurrentVehicle(Vehicle_Controller vehicle)
    {
        _currentVehicle = vehicle;
    }



    // Current Characters Control
    private List<GameObject> _currentCharacters = new();
    public List<GameObject> currentCharacters => _currentCharacters;

    public void Track_CurrentCharacter(GameObject character)
    {
        _currentCharacters.Add(character);
    }
    public void UnTrack_CurrentCharacter(GameObject character)
    {
        _currentCharacters.Remove(character);
    }
    public void Spawn_Character(int arrayNum, Vector2 spawnPosition)
    {
        GameObject character = Instantiate(dataController.characters[arrayNum], spawnPosition, Quaternion.identity);
        character.transform.parent = _characterFile;
    }



    // Current Stations Control
    private List<Station_Controller> _currentStations = new();
    public List<Station_Controller> currentStations => _currentStations;

    public void Track_CurrentStation(Station_Controller station)
    {
        _currentStations.Add(station);
    }
    public void UnTrack_CurrentStation(Station_Controller station)
    {
        _currentStations.Remove(station);
    }
    public Station_Controller Spawn_Station(int arrayNum, Vector2 spawnPosition)
    {
        GameObject station = Instantiate(dataController.stationPrefabs[arrayNum], spawnPosition, Quaternion.identity);
        station.transform.parent = _stationFile;

        if (station.TryGetComponent(out Station_Controller stationController) == false) return null;
        return stationController;
    }

    /// <summary>
    /// sorts current stations list from closest to farthest from target
    /// </summary>
    public void Sort_CurrentStation_fromClosest(Transform target)
    {
        _currentStations.Sort((x, y) =>
        Vector2.Distance(x.transform.position, target.position)
        .CompareTo
        (Vector2.Distance(y.transform.position, target.position)));
    }




    // Current Archive and BookMark Foods Control
    private List<Food_ScrObj> _archiveFoods = new();
    public List<Food_ScrObj> archiveFoods => _archiveFoods;

    public bool Is_ArchivedFood(Food_ScrObj food)
    {
        if (food == null) return false;

        for (int i = 0; i < _archiveFoods.Count; i++)
        {
            if (food == _archiveFoods[i]) return true;
        }
        return false;
    }
    public void AddFood_toArhive(Food_ScrObj food)
    {
        if (food == null) return;

        if (Is_ArchivedFood(food) == true) return;

        _archiveFoods.Add(food);
    }

    private List<Food_ScrObj> _bookmarkedFoods = new();
    public List<Food_ScrObj> bookmarkedFoods => _bookmarkedFoods;

    public bool Is_BookmarkedFood(Food_ScrObj food)
    {
        if (food == null) return false;

        for (int i = 0; i < _bookmarkedFoods.Count; i++)
        {
            if (food == _bookmarkedFoods[i]) return true;
        }
        return false;
    }
    public void AddFood_toBookmark(Food_ScrObj food)
    {
        if (food == null) return;

        _bookmarkedFoods.Add(food);
    }
    public void RemoveFood_fromBookmark(Food_ScrObj food)
    {
        if (food == null) return;

        _bookmarkedFoods.Remove(food);
    }
}