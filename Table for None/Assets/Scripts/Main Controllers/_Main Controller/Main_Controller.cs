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

    private MainController_Data _data;
    public MainController_Data data => _data;

    
    [SerializeField] private bool _demoBuild;
    public bool demoBuild => _demoBuild;


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
    
    
    // Editor
    [HideInInspector] public Food_ScrObj foodToAdd;
    [HideInInspector] public int amountToAdd;
    
    [HideInInspector] public Station_ScrObj editStation;
    [HideInInspector] public bool lockStation;
    
    [HideInInspector] public Food_ScrObj archiveFood;
    [HideInInspector] public bool unlockIngredient;


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
        Save_CurrentStations();
        
        ES3.Save("Main_Controller/MainController_Data", _data);
    }

    public void Load_Data()
    {
        _data = ES3.Load("Main_Controller/MainController_Data", new MainController_Data());

        Load_CurrentStations();
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
        _data.stationLoadDatas.Clear();
        
        for (int i = 0; i < _currentStations.Count; i++)
        {
            if (_currentStations[i].movement != null && _currentStations[i].movement.enabled == true) continue;
            StationData stationData = new(_currentStations[i].data);

            FoodData_Controller stationIcon = _currentStations[i].Food_Icon();
            List<FoodData> foodDatas = stationIcon != null ? stationIcon.AllDatas() : null;
            
            _data.stationLoadDatas.Add(new(stationData, foodDatas, _worldMap.data.currentData));
        }
    }
    
    private void Load_Station(Station_LoadData stationLoadData)
    {
        StationData stationData = stationLoadData.stationData;

        Station_Controller station = Spawn_Station(stationData.stationScrObj, stationData.position);
        station.Set_Data(stationData);
        
        Station_Movement movement = station.movement;
        
        if (movement != null)
        {
            movement.Load_Position();
            station.transform.position = movement.Offset_Position();
        }
        else _data.Claim_Position(station.transform.position);
            
        // load food data
        FoodData_Controller stationIcon = station.Food_Icon();
        if (stationLoadData.foodDatas == null || stationIcon == null) return;
            
        stationIcon.Update_AllDatas(stationLoadData.foodDatas);
        stationIcon.Show_Icon();
        stationIcon.Show_Condition();
    }
    private void Load_CurrentStations()
    {
        List<Station_LoadData> stationLoadDatas = _data.stationLoadDatas;

        // default station load
        for (int i = stationLoadDatas.Count - 1; i >= 0; i--)
        {
            if (stationLoadDatas[i].stationData.stationScrObj.overlapPlaceable) continue;

            Load_Station(stationLoadDatas[i]);
            stationLoadDatas.RemoveAt(i);
        }

        // overlap station load
        foreach (Station_LoadData data in stationLoadDatas)
        {
            Load_Station(data);
        }
    }

    public Station_Controller Spawn_Station(Station_ScrObj stationScrObj, Vector2 spawnPosition)
    {
        GameObject spawnStation = Instantiate(stationScrObj.prefab, spawnPosition, Quaternion.identity);

        if (!spawnStation.TryGetComponent(out Station_Controller stationController)) return null;
        return stationController;
    }
    public void Track_CurrentStation(Station_Controller station)
    {
        _currentStations.Add(station);

        station.transform.SetParent(_stationFile);
    }

    public int Destroy_AllStations()
    {
        int destroyCount = 0;

        for (int i = _currentStations.Count - 1; i >= 0; i--)
        {
            _data.claimedPositions.Remove(currentStations[i].transform.position);
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

    /// <returns>
    /// only stations that are placed
    /// </returns>
    public List<Station_Controller> CurrentStations()
    {
        List<Station_Controller> placedStations = new();
        
        for (int i = 0; i < _currentStations.Count; i++)
        {
            if (_currentStations[i].movement == null) continue;
            if (_currentStations[i].movement.enabled == true) continue;
            
            placedStations.Add(_currentStations[i]);
        }
        
        return placedStations;
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
    /// <returns>
    /// All searchStations in _currentStations
    /// </returns>
    public List<Station_Controller> CurrentStations(Vector2 searchPosition)
    {
        List<Station_Controller> searchStations = new();
        
        for (int i = 0; i < _currentStations.Count; i++)
        {
            Station_Movement movement = _currentStations[i].movement;
            if (movement == null || movement.enabled) continue;
            
            if (searchPosition != (Vector2)_currentStations[i].data.position) continue;
            
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
    
    
    // // Food Bookmarking
    public Action OnFoodBookmark;
    
    public void Add_Bookmark(Food_ScrObj food)
    {
        if (food == null) return;

        _data.bookmarkedFoods.Add(food);
        _data.Remove_DuplicateBookmarks();

        OnFoodBookmark?.Invoke();
    }
    public void Remove_Bookmark(Food_ScrObj food)
    {
        if (food == null) return;

        _data.bookmarkedFoods.Remove(food);
        _data.Remove_DuplicateBookmarks();

        OnFoodBookmark?.Invoke();
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(Main_Controller))]
public class Main_Controller_Inspector : Editor
{
    private SerializedProperty foodToAddProp;
    private SerializedProperty amountToAddProp;
    
    private SerializedProperty editStationProp;
    private SerializedProperty lockStationProp;
    
    private SerializedProperty _archiveFoodProp;
    private SerializedProperty _unlockIngredientProp;
    
    private void OnEnable()
    {
        foodToAddProp = serializedObject.FindProperty("foodToAdd");
        amountToAddProp = serializedObject.FindProperty("amountToAdd");
        
        editStationProp = serializedObject.FindProperty("editStation");
        lockStationProp = serializedObject.FindProperty("lockStation");
        
        _archiveFoodProp = serializedObject.FindProperty("archiveFood");
        _unlockIngredientProp = serializedObject.FindProperty("unlockIngredient");
    }
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

        GUILayout.Space(60);

        if (GUILayout.Button("Add Gold"))
        {
            GoldSystem.instance.Update_CurrentAmount(50);
        }
        
        GUILayout.Space(30);
        
        if (GUILayout.Button("Add New Page of Slots"))
        {
            FoodMenu_Controller foodMenu = Main_Controller.instance.currentVehicle.menu.foodMenu;
            foodMenu.controller.slotsController.AddNewPage_ItemSlotDatas(foodMenu.ItemSlot_Datas());
        }
        
        GUILayout.BeginHorizontal();
        
        EditorGUILayout.PropertyField(foodToAddProp, GUIContent.none);
        Food_ScrObj foodToAdd = (Food_ScrObj)foodToAddProp.objectReferenceValue;

        EditorGUILayout.PropertyField(amountToAddProp, GUIContent.none);
        int amountToAdd = amountToAddProp.intValue;
        
        GUILayout.EndHorizontal();
        
        if (GUILayout.Button("Add Food"))
        {
            FoodMenu_Controller foodMenu = Main_Controller.instance.currentVehicle.menu.foodMenu;
            foodMenu.Add_FoodItem(foodToAdd, amountToAdd);
        }

        if (GUILayout.Button("Remove Food"))
        {
            FoodMenu_Controller foodMenu = Main_Controller.instance.currentVehicle.menu.foodMenu;
            foodMenu.Remove_FoodItem(foodToAdd, amountToAdd);
        }
        
        GUILayout.Space(30);

        if (GUILayout.Button("Add New Page of Slots"))
        {
            StationMenu_Controller stationMenu = Main_Controller.instance.currentVehicle.menu.stationMenu;
            stationMenu.controller.slotsController.AddNewPage_ItemSlotDatas(stationMenu.ItemSlot_Datas());
        }

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.PropertyField(editStationProp, GUIContent.none);
        Station_ScrObj editStation = (Station_ScrObj)editStationProp.objectReferenceValue;

        EditorGUILayout.PropertyField(lockStationProp, GUIContent.none);
        bool lockStation = lockStationProp.boolValue;

        if (GUILayout.Button("Add Station"))
        {
            StationMenu_Controller stationMenu = Main_Controller.instance.currentVehicle.menu.stationMenu;
            stationMenu.Toggle_DataLock(stationMenu.Add_StationItem(editStation, 1), lockStation);
        }

        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Remove Station"))
        {
            StationMenu_Controller stationMenu = Main_Controller.instance.currentVehicle.menu.stationMenu;
            stationMenu.Remove_StationItem(editStation);
        }
        
        GUILayout.Space(30);
        
        EditorGUILayout.PropertyField(_archiveFoodProp, GUIContent.none);
        Food_ScrObj archiveFood = (Food_ScrObj)_archiveFoodProp.objectReferenceValue;

        EditorGUILayout.PropertyField(_unlockIngredientProp, GUIContent.none);
        bool unlockIngredient = _unlockIngredientProp.boolValue;

        if (GUILayout.Button("Archive Food"))
        {
            ArchiveMenu_Controller archiveMenu = Main_Controller.instance.currentVehicle.menu.archiveMenu;
            
            if (unlockIngredient)
            {
                archiveMenu.Unlock_FoodIngredient(archiveFood, 1);
            }

            bool rawFood = Main_Controller.instance.dataController.Is_RawFood(archiveFood);
            archiveMenu.Unlock_BookmarkToggle(archiveMenu.Archive_Food(archiveFood), rawFood || !archiveMenu.FoodIngredient_Unlocked(archiveFood));
            archiveMenu.Unlock_FoodIngredient(archiveFood, 0);
        }
        
        serializedObject.ApplyModifiedProperties();
    }
}
#endif