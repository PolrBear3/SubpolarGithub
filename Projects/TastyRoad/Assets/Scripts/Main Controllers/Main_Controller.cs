using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main_Controller : MonoBehaviour, ISaveLoadable
{
    [HideInInspector] public Data_Controller dataController;

    public GlobalTime_Controller globalTime => _globalTime;
    private GlobalTime_Controller _globalTime;

    [SerializeField] private Camera _mainCamera;

    [SerializeField] private RectTransform _openingCurtain;

    [Header("File Locations")]
    [SerializeField] private Transform _locationFile;
    [SerializeField] private Transform _characterFile;
    [SerializeField] private Transform _stationFile;

    public static bool gamePaused;
    public static bool orderOpen;

    public static int currentGoldCoin;
    public static int currentStationCoin;
    public static int currentGasCoin;



    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Data_Controller dataController)) { this.dataController = dataController; }
        if (gameObject.TryGetComponent(out GlobalTime_Controller globalTime)) { _globalTime = globalTime; }
    }

    private void Start()
    {
        LeanTween.alpha(_openingCurtain, 0f, 1f);
    }



    // ISaveLoadable
    public void Save_Data()
    {
        ES3.Save("currentGoldCoin", currentGoldCoin);
        ES3.Save("currentStationCoin", currentStationCoin);

        ES3.Save("_claimedPositions", _claimedPositions);

        Save_ArchivedFood();
        Save_BookmarkedFood();

        Save_CurrentStations();
    }

    public void Load_Data()
    {
        currentGoldCoin = ES3.Load("currentGoldCoin", currentGoldCoin);
        currentStationCoin = ES3.Load("currentStationCoin", currentStationCoin);

        _claimedPositions = ES3.Load("_claimedPositions", _claimedPositions);

        Load_ArchivedFood();
        Load_bookmarkedFood();

        Load_CurrentStations();
    }



    /// <summary>
    /// (-1, 0, 1) horizontal(left, right), vertical(top bottom), position x and y(local position)
    /// </summary>
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
    /// <summary>
    /// left is -1, right is 1
    /// </summary>
    public Vector2 OuterCamera_Position(int leftRight)
    {
        // left
        if (leftRight <= 0) return OuterCamera_Position(-1, 0, -2, -3);

        // right
        else return OuterCamera_Position(1, 0, 2, -3);
    }



    /// <summary>
    /// Changes inserted sprite to target transparency
    /// </summary>
    public static void Change_SpriteAlpha(SpriteRenderer sr, float alpha)
    {
        Color color = sr.color;
        color.a = alpha;
        sr.color = color;
    }

    /// <returns>
    /// True if inserted percentage amount is activated, False if not activated
    /// </returns>
    public static bool Percentage_Activated(float percentage)
    {
        float comparePercentage = Mathf.Round(Random.Range(0f, 100f)) * 1f;

        return percentage >= comparePercentage;
    }

    /// <returns>
    /// Mathf Round floats of inserted vector x and y value
    /// </returns>
    public static Vector2 SnapPosition(Vector2 position)
    {
        float snapX = (float)Mathf.Round(position.x);
        float snapY = (float)Mathf.Round(position.y);

        return new Vector2(snapX, snapY);
    }

    /// <returns>
    /// Random point inside the boundary of inserted sprite renderer
    /// </returns>
    public static Vector2 Random_AreaPoint(SpriteRenderer area)
    {
        Vector2 centerPosition = area.bounds.center;

        float randX = Random.Range(centerPosition.x - area.bounds.extents.x, centerPosition.x + area.bounds.extents.x);
        float randY = Random.Range(centerPosition.y - area.bounds.extents.y, centerPosition.y + area.bounds.extents.y);

        return new Vector2(randX, randY);
    }



    // Position Control
    private List<Vector2> _claimedPositions = new();

    public void Claim_Position(Vector2 position)
    {
        if (Position_Claimed(position)) return;
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

    private void Save_CurrentStations()
    {
        List<StationData> saveData = new();
        List<FoodData> foodData = new();

        for (int i = 0; i < _currentStations.Count; i++)
        {
            if (_currentStations[i].movement != null && _currentStations[i].movement.enabled == true) continue;

            saveData.Add(new StationData(_currentStations[i]));

            if (_currentStations[i].Food_Icon() == null)
            {
                foodData.Add(null);
                continue;
            }
            foodData.Add(new FoodData(_currentStations[i].Food_Icon().currentFoodData));
        }

        ES3.Save("_currentStations", saveData);
        ES3.Save("_currentStations" + "foodData", foodData);
    }
    private void Load_CurrentStations()
    {
        List<StationData> loadData = ES3.Load("_currentStations", new List<StationData>());
        List<FoodData> foodData = ES3.Load("_currentStations" + "foodData", new List<FoodData>());

        for (int i = 0; i < loadData.Count; i++)
        {
            Station_Controller station = Spawn_Station(loadData[i].stationID, loadData[i].position);

            if (_currentStations[i].movement != null)
            {
                station.movement.Load_Position();
            }

            if (foodData[i] == null) continue;

            FoodData_Controller foodIcon = station.Food_Icon();

            foodIcon.currentFoodData = foodData[i];
            foodIcon.Load_FoodData();
            foodIcon.stateBoxController.Update_StateBoxes();
        }
    }

    public void Track_CurrentStation(Station_Controller station)
    {
        _currentStations.Add(station);
    }
    public void UnTrack_CurrentStation(Station_Controller station)
    {
        _currentStations.Remove(station);
    }

    public Station_Controller Spawn_Station(int id, Vector2 spawnPosition)
    {
        List<Station_ScrObj> allStations = dataController.stations;
        Station_Controller targetStation = null;

        for (int i = 0; i < allStations.Count; i++)
        {
            if (id != allStations[i].id) continue;

            GameObject spawnStation = Instantiate(allStations[i].prefab, spawnPosition, Quaternion.identity);
            spawnStation.transform.parent = _stationFile;

            if (!spawnStation.TryGetComponent(out Station_Controller stationController)) return null;
            targetStation = stationController;
        }

        return targetStation;
    }
    public Station_Controller Station(Vector2 searchPosition)
    {
        for (int i = 0; i < currentStations.Count; i++)
        {
            if (searchPosition.x != currentStations[i].transform.position.x) continue;
            if (searchPosition.y != currentStations[i].transform.position.y) continue;

            return currentStations[i];
        }

        return null;
    }

    /// <returns>
    /// Current stations that are retrievable by selecting from menu
    /// </returns>
    public List<Station_Controller> Retrievable_Stations()
    {
        List<Station_Controller> retrievableStations = new();

        for (int i = 0; i < _currentStations.Count; i++)
        {
            if (_currentStations[i].stationScrObj.unRetrievable) continue;
            retrievableStations.Add(_currentStations[i]);
        }

        return retrievableStations;
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

    private void Save_ArchivedFood()
    {
        List<int> foodIDs = new();

        for (int i = 0; i < _archiveFoods.Count; i++)
        {
            foodIDs.Add(_archiveFoods[i].id);
        }

        ES3.Save("Main_Controller/_archiveFoods/foodIDs", foodIDs);
    }
    private void Load_ArchivedFood()
    {
        List<int> foodIDs = new();

        ES3.Load("Main_Controller/_archiveFoods/foodIDs", foodIDs);

        for (int i = 0; i < foodIDs.Count; i++)
        {
            _archiveFoods.Add(dataController.Food(foodIDs[i]));
        }
    }

    public void AddFood_toArhive(Food_ScrObj food)
    {
        if (food == null) return;

        if (Is_ArchivedFood(food) == true) return;

        _archiveFoods.Add(food);
    }

    public bool Is_ArchivedFood(Food_ScrObj food)
    {
        if (food == null) return false;

        for (int i = 0; i < _archiveFoods.Count; i++)
        {
            if (food == _archiveFoods[i]) return true;
        }
        return false;
    }



    private List<Food_ScrObj> _bookmarkedFoods = new();
    public List<Food_ScrObj> bookmarkedFoods => _bookmarkedFoods;

    private void Save_BookmarkedFood()
    {
        List<int> foodIDs = new();

        for (int i = 0; i < _bookmarkedFoods.Count; i++)
        {
            foodIDs.Add(_bookmarkedFoods[i].id);
        }

        ES3.Save("Main_Controller/_bookmarkedFoods/foodIDs", foodIDs);
    }
    private void Load_bookmarkedFood()
    {
        List<int> foodIDs = new();

        ES3.Load("Main_Controller/_bookmarkedFoods/foodIDs", foodIDs);

        for (int i = 0; i < foodIDs.Count; i++)
        {
            _bookmarkedFoods.Add(dataController.Food(foodIDs[i]));
        }
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

    public bool Is_BookmarkedFood(Food_ScrObj food)
    {
        if (food == null) return false;

        for (int i = 0; i < _bookmarkedFoods.Count; i++)
        {
            if (food == _bookmarkedFoods[i]) return true;
        }
        return false;
    }
}