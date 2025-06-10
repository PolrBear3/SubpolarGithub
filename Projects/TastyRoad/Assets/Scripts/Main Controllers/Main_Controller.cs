using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEditor;

public class Main_Controller : MonoBehaviour, ISaveLoadable
{
    public static Main_Controller instance;


    [Space(20)]
    [SerializeField] private Camera_Controller _cameraController;
    public Camera_Controller cameraController => _cameraController;

    [Space(20)] 
    [SerializeField] private GlobalLight_Controller _globalLightController;
    public GlobalLight_Controller globalLightController => _globalLightController;

    [SerializeField] private Volume _globalVolume;
    public Volume globalVolume => _globalVolume;


    [Space(20)]
    [SerializeField] private Data_Controller _dataController;
    public Data_Controller dataController => _dataController;

    [SerializeField] private TransitionCanvas_Controller _transitionCanvas;
    public TransitionCanvas_Controller transitionCanvas => _transitionCanvas;

    [SerializeField] private DialogSystem _dialogSystem;
    public DialogSystem dialogSystem => _dialogSystem;

    [SerializeField] private WorldMap_Controller _worldMap;
    public WorldMap_Controller worldMap => _worldMap;

    [SerializeField] private globaltime _globalTime;
    public globaltime globalTime => _globalTime;

    [SerializeField] private SubLocations_Controller _subLocation;
    public SubLocations_Controller subLocation => _subLocation;


    [Space(20)]
    [SerializeField] private Transform _locationFile;
    public Transform locationFile => _locationFile;

    [SerializeField] private Transform _characterFile;
    public Transform characterFile => _characterFile;

    [SerializeField] private Transform _stationFile;

    [SerializeField] private Transform _otherFile;
    public Transform otherFile => _otherFile;


    // MonoBehaviour
    private void Awake()
    {
        instance = this;
        Application.targetFrameRate = 60;
    }


    // ISaveLoadable
    public void Save_Data()
    {
        UnClaim_CustomPositions();
        ES3.Save("_claimedPositions", _claimedPositions);

        Save_BookmarkedFood();
        Save_CurrentStations();
    }

    public void Load_Data()
    {
        _claimedPositions = ES3.Load("_claimedPositions", _claimedPositions);

        Load_bookmarkedFood();
        Load_CurrentStations();
    }


    // Tools
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
    public void Change_SpriteAlpha(SpriteRenderer sr, float alpha)
    {
        Color color = sr.color;
        color.a = alpha;
        sr.color = color;
    }

    public void Change_ImageAlpha(Image image, float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }


    /// <returns>
    /// True if inserted percentage amount is activated, False if not activated
    /// </returns>
    public bool Percentage_Activated(float percentage)
    {
        float comparePercentage = Mathf.Round(UnityEngine.Random.Range(0f, 100f)) * 1f;
        return percentage >= comparePercentage;
    }

    public bool Percentage_Activated(float rangeValue, float percentage)
    {
        float comparePercentage = Mathf.Round(UnityEngine.Random.Range(0f, rangeValue)) * 1f;
        return percentage >= comparePercentage;
    }


    /// <returns>
    /// Mathf Round floats of inserted vector x and y value
    /// </returns>
    public Vector2 SnapPosition(Vector2 position)
    {
        float snapX = (float)Mathf.Round(position.x);
        float snapY = (float)Mathf.Round(position.y);

        return new Vector2(snapX, snapY);
    }
    public Vector2 SnapPosition(Vector2 position, Bounds bounds)
    {
        // Round the position to the nearest integer
        int snapX = Mathf.RoundToInt(position.x);
        int snapY = Mathf.RoundToInt(position.y);

        // Clamp the snapped position within the integer bounds
        snapX = Mathf.Clamp(snapX, Mathf.CeilToInt(bounds.min.x), Mathf.FloorToInt(bounds.max.x));
        snapY = Mathf.Clamp(snapY, Mathf.CeilToInt(bounds.min.y), Mathf.FloorToInt(bounds.max.y));

        return new Vector2(snapX, snapY);
    }

    /// <returns>
    /// Random point inside the boundary of inserted sprite renderer
    /// </returns>
    public Vector2 Random_AreaPoint(SpriteRenderer area)
    {
        Vector2 centerPosition = area.bounds.center;

        float randX = UnityEngine.Random.Range(centerPosition.x - area.bounds.extents.x, centerPosition.x + area.bounds.extents.x);
        float randY = UnityEngine.Random.Range(centerPosition.y - area.bounds.extents.y, centerPosition.y + area.bounds.extents.y);

        return new Vector2(randX, randY);
    }


    // Position Control
    private List<Vector2> _claimedPositions = new();
    public List<Vector2> claimedPositions => _claimedPositions;

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

    /// <summary>
    /// Before saving, UnClaim custom positions from all Custom_positionClaimer
    /// </summary>
    private void UnClaim_CustomPositions()
    {
        Custom_PositionClaimer[] claimers = FindObjectsOfType<Custom_PositionClaimer>();

        foreach (Custom_PositionClaimer claimer in claimers)
        {
            claimer.UnClaim_CurrentPositions();
        }
    }


    // Current Location Control
    private Location_Controller _currentLocation;
    public Location_Controller currentLocation => _currentLocation;

    public void Track_CurrentLocaiton(Location_Controller location)
    {
        _currentLocation = location;
    }

    public Location_Controller Set_Location(WorldMap_Data data)
    {
        WorldData worldData = _dataController.World_Data(data.worldNum);
        Location_ScrObj location = worldData.Location_ScrObj(data.locationNum);

        GameObject setLocation = Instantiate(location.locationPrefab, Vector2.zero, Quaternion.identity);
        setLocation.transform.parent = _locationFile;

        return setLocation.GetComponent<Location_Controller>();
    }


    // Current Sub Locations and Other Interactables



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
        // exported station datas, all food datas
        Dictionary<StationData, List<FoodData>> stationDatas = new();

        for (int i = 0; i < _currentStations.Count; i++)
        {
            if (_currentStations[i].movement != null && _currentStations[i].movement.enabled == true) continue;

            StationData stationData = new(_currentStations[i].data);
            stationDatas.Add(stationData, new());

            if (_currentStations[i].Food_Icon() == null || _currentStations[i].Food_Icon().hasFood == false) continue;

            stationDatas[stationData] = new(_currentStations[i].Food_Icon().AllDatas());
        }

        ES3.Save("Main_Controller/stationDatas", stationDatas);
    }
    private void Load_CurrentStations()
    {
        if (ES3.KeyExists("Main_Controller/stationDatas") == false) return;

        // exported station datas, all food datas
        Dictionary<StationData, List<FoodData>> stationDatas = ES3.Load("Main_Controller/stationDatas", new Dictionary<StationData, List<FoodData>>());

        foreach (var data in stationDatas)
        {
            StationData stationData = new(data.Key);
            List<FoodData> foodDatas = new(data.Value);

            // spawn saved stations
            Station_Controller station = Spawn_Station(stationData.stationScrObj, stationData.position);

            // load station data
            station.Set_Data(stationData);

            // deactivate and set station if a movement script is attached
            Station_Movement movement = station.movement;

            if (movement != null)
            {
                movement.Load_Position();
            }

            // load food data
            if (station.Food_Icon() == null) continue;

            station.Food_Icon().Update_AllDatas(foodDatas);
            station.Food_Icon().Show_Icon();
            station.Food_Icon().Show_Condition();
        }
    }

    public void Track_CurrentStation(Station_Controller station)
    {
        _currentStations.Add(station);

        station.transform.SetParent(_stationFile);
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
            UnClaim_Position(currentStations[i].transform.position);
            _currentStations[i].Destroy_Station();

            destroyCount++;
        }

        return destroyCount;
    }

    public bool Is_StationArea(Vector2 areaPoint)
    {
        for (int i = 0; i < _currentStations.Count; i++)
        {
            if (_currentStations[i].isRoamArea) continue;

            // get station sprite render bound
            Bounds stationArea = _currentStations[i].spriteRenderer.bounds;

            // check if areaPoint is inside stationArea
            if (stationArea.Contains(areaPoint)) return true;
        }

        return false;
    }

    public Station_Controller Spawn_Station(Station_ScrObj stationScrObj, Vector2 spawnPosition)
    {
        GameObject spawnStation = Instantiate(stationScrObj.prefab, spawnPosition, Quaternion.identity);

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


    // Main Game Global BookMark Foods Control
    private List<Food_ScrObj> _bookmarkedFoods = new();
    public List<Food_ScrObj> bookmarkedFoods => _bookmarkedFoods;

    public Action OnFoodBookmark;

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

        OnFoodBookmark?.Invoke();
    }
    public void RemoveFood_fromBookmark(Food_ScrObj food)
    {
        if (food == null) return;

        _bookmarkedFoods.Remove(food);
        RemoveDuplicates_fromBookmark();

        OnFoodBookmark?.Invoke();
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

    public bool Food_BookmarkedFood()
    {
        return _bookmarkedFoods.Count > 0;
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(Main_Controller))]
public class Main_Controller_Inspector : Editor
{
    //
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

        GUILayout.Space(60);

        if (GUILayout.Button("Test Button"))
        {
            GoldSystem.instance.Update_CurrentAmount(50);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif