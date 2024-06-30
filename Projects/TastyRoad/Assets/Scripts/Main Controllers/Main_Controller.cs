using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main_Controller : MonoBehaviour, ISaveLoadable
{
    [SerializeField] private Camera_Controller _cameraController;
    public Camera_Controller cameraController => _cameraController;

    [Header("")]
    [SerializeField] private Data_Controller _dataController;
    public Data_Controller dataController => _dataController;

    [SerializeField] private TransitionCanvas_Controller _transitionCanvas;
    public TransitionCanvas_Controller transitionCanvas => _transitionCanvas;

    [SerializeField] private DialogSystem _dialogSystem;
    public DialogSystem dialogSystem => _dialogSystem;

    [SerializeField] private WorldMap_Controller _worldMap;
    public WorldMap_Controller worldMap => _worldMap;

    [SerializeField] private GlobalTime_Controller _globalTime;
    public GlobalTime_Controller globalTime => _globalTime;

    [SerializeField] private SubLocations_Controller _subLocation;
    public SubLocations_Controller subLocation => _subLocation;

    [Header("")]
    [SerializeField] private Transform _locationFile;
    [SerializeField] private Transform _characterFile;
    [SerializeField] private Transform _stationFile;


    public static bool gamePaused;
    public static bool orderOpen;


    public delegate void Event();

    public static event Event OrderOpen_ToggleEvent;

    public static event Event TestButton1Event;
    public static event Event TestButton2Event;
    public static event Event TestButton3Event;


    // GameObject.FindGameObjectWithTag("MainController").GetComponent<Main_Controller>();


    // MonoBehaviour
    private void Start()
    {
        Application.targetFrameRate = 60;
    }


    // ISaveLoadable
    public void Save_Data()
    {
        gamePaused = false;
        orderOpen = false;

        ES3.Save("_claimedPositions", _claimedPositions);

        Save_ArchivedFood();
        Save_BookmarkedFood();

        Save_CurrentLocationData();
        Save_CurrentStations();
    }

    public void Load_Data()
    {
        _claimedPositions = ES3.Load("_claimedPositions", _claimedPositions);

        Load_ArchivedFood();
        Load_bookmarkedFood();

        Load_CurrentLocationData();
        Load_CurrentStations();
    }


    // Test Buttons
    public void TestButton1()
    {
        TestButton1Event?.Invoke();
    }

    public void TestButton2()
    {
        TestButton2Event?.Invoke();
    }

    public void TestButton3()
    {
        TestButton3Event?.Invoke();
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

        Vector2 cameraPosition = cameraController.mainCamera.ViewportToWorldPoint(new Vector2(horizontalPos, verticalPos));

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

    public void ResetAll_ClaimedPositions()
    {
        _claimedPositions.Clear();
    }

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


    // Current Golden Nugget Control
    public int GoldenNugget_Amount()
    {
        FoodMenu_Controller foodMenu = _currentVehicle.menu.foodMenu;
        return foodMenu.FoodAmount(dataController.goldenNugget);
    }


    public int Add_GoldenNugget(int addAmount)
    {
        FoodMenu_Controller foodMenu = _currentVehicle.menu.foodMenu;
        return foodMenu.Add_FoodItem(dataController.goldenNugget, addAmount);
    }

    public void Remove_GoldenNugget(int removeAmount)
    {
        FoodMenu_Controller foodMenu = _currentVehicle.menu.foodMenu;
        foodMenu.Remove_FoodItem(dataController.goldenNugget, removeAmount);
    }


    // Order Open Control
    public static void OrderOpen_Toggle(bool toggleOn)
    {
        orderOpen = toggleOn;
        OrderOpen_ToggleEvent?.Invoke();
    }


    // Current Location Control
    private Location_Controller _currentLocation;
    public Location_Controller currentLocation => _currentLocation;

    private LocationData _savedLocationData;
    public LocationData savedLocationData => _savedLocationData;

    private void Save_CurrentLocationData()
    {
        ES3.Save("Main_Controller/_currentLocationData", _savedLocationData);
    }
    public void Load_CurrentLocationData()
    {
        if (ES3.KeyExists("Main_Controller/_currentLocationData") == false) return;

        _savedLocationData = ES3.Load("Main_Controller/_currentLocationData", _savedLocationData);
    }

    public void Track_CurrentLocaiton(Location_Controller location)
    {
        _currentLocation = location;
        _savedLocationData = location.currentData;

        Save_CurrentLocationData();
    }

    public Location_Controller Set_Location(int worldNum, int locationNum)
    {
        List<Location_ScrObj> allLocations = _dataController.locations;

        for (int i = 0; i < allLocations.Count; i++)
        {
            if (worldNum != allLocations[i].worldNum) continue;
            if (locationNum != allLocations[i].locationNum) continue;

            GameObject location = Instantiate(allLocations[i].locationPrefab, Vector2.zero, Quaternion.identity);
            location.transform.parent = _locationFile;

            return location.GetComponent<Location_Controller>();
        }

        return null;
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

    public void Destroy_AllCharacters()
    {
        for (int i = _currentCharacters.Count - 1; i >= 0; i--)
        {
            Destroy(_currentCharacters[i]);
        }
    }

    public GameObject Spawn_Character(int arrayNum, Vector2 spawnPosition)
    {
        GameObject character = Instantiate(dataController.characters[arrayNum], spawnPosition, Quaternion.identity);
        character.transform.parent = _characterFile;

        return character;
    }

    public Player_Controller Player()
    {
        for (int i = 0; i < _currentCharacters.Count; i++)
        {
            if (!_currentCharacters[i].TryGetComponent(out Player_Controller player)) continue;
            return player;
        }

        return null;
    }
    public List<NPC_Controller> All_NPCs()
    {
        List<NPC_Controller> allNPCs = new();

        for (int i = 0; i < _currentCharacters.Count; i++)
        {
            if (!_currentCharacters[i].TryGetComponent(out NPC_Controller npc)) continue;
            allNPCs.Add(npc);
        }

        return allNPCs;
    }


    // Current Stations Control
    private List<Station_Controller> _currentStations = new();
    public List<Station_Controller> currentStations => _currentStations;

    private void Save_CurrentStations()
    {
        List<StationData> stationDatas = new();
        List<FoodData> stationFoodDatas = new();

        for (int i = 0; i < _currentStations.Count; i++)
        {
            // check if station is exported and not placed
            if (_currentStations[i].movement != null && _currentStations[i].movement.enabled == true) continue;

            stationDatas.Add(new StationData(_currentStations[i]));

            // food data save
            if (_currentStations[i].Food_Icon() == null || _currentStations[i].Food_Icon().hasFood == false)
            {

                stationFoodDatas.Add(null);
                continue;
            }

            stationFoodDatas.Add(new FoodData(_currentStations[i].Food_Icon().currentData));
        }

        ES3.Save("Main_Controller/stationDatas", stationDatas);
        ES3.Save("Main_Controller/stationFoodDatas", stationFoodDatas);
    }
    private void Load_CurrentStations()
    {
        if (ES3.KeyExists("Main_Controller/stationDatas") == false) return; 

        List<StationData> stationDatas = ES3.Load<List<StationData>>("Main_Controller/stationDatas");
        List<FoodData> stationFoodDatas = ES3.Load<List<FoodData>>("Main_Controller/stationFoodDatas");

        for (int i = 0; i < stationDatas.Count; i++)
        {
            // spawn saved stations
            Station_Controller station = Spawn_Station(stationDatas[i].stationScrObj, stationDatas[i].position);

            // deactivate and set station if is has a movement script attached
            if (station.movement != null)
            {
                station.movement.Load_Position();
            }

            // food data load
            if (_currentStations[i].Food_Icon() == null) continue;

            _currentStations[i].Food_Icon().Set_CurrentData(stationFoodDatas[i]);
            _currentStations[i].Food_Icon().Show_Icon();
            _currentStations[i].Food_Icon().Show_Condition();
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

    public int Destroy_AllStations()
    {
        int destroyCount = 0;

        for (int i = _currentStations.Count - 1; i >= 0; i--)
        {
            _currentStations[i].Destroy_Station();
            destroyCount++;
        }

        return destroyCount;
    }

    public Station_Controller Spawn_Station(Station_ScrObj stationScrObj, Vector2 spawnPosition)
    {
        GameObject spawnStation = Instantiate(stationScrObj.prefab, spawnPosition, Quaternion.identity);
        spawnStation.transform.parent = _stationFile;

        if (!spawnStation.TryGetComponent(out Station_Controller stationController)) return null;
        return stationController;
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
    /// only stations that have movement controller, if false
    /// </returns>
    public List<Station_Controller> CurrentStations(bool allStations)
    {
        if (allStations == true) return _currentStations;

        List<Station_Controller> retrievables = new();

        for (int i = 0; i < _currentStations.Count; i++)
        {
            if (_currentStations[i].movement == null) continue;
            retrievables.Add(_currentStations[i]);
        }

        return retrievables;
    }
    /// <returns>
    /// All searchStations in _currentStations
    /// </returns>
    public List<Station_Controller> CurrentStations(Station_ScrObj searchStation)
    {
        List<Station_Controller> searchStations = new();

        for (int i = 0; i < _currentStations.Count; i++)
        {
            if (searchStation != _currentStations[i].stationScrObj) continue;
            searchStations.Add(_currentStations[i]);
        }

        return searchStations;
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

        foodIDs = ES3.Load("Main_Controller/_bookmarkedFoods/foodIDs", foodIDs);

        for (int i = 0; i < foodIDs.Count; i++)
        {
            _bookmarkedFoods.Add(dataController.Food(foodIDs[i]));
        }
    }

    private void RemoveDuplicates_fromBookmark()
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

    public void AddFood_toBookmark(Food_ScrObj food)
    {
        if (food == null) return;

        _bookmarkedFoods.Add(food);

        RemoveDuplicates_fromBookmark();
    }
    public void RemoveFood_fromBookmark(Food_ScrObj food)
    {
        if (food == null) return;

        _bookmarkedFoods.Remove(food);

        RemoveDuplicates_fromBookmark();
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