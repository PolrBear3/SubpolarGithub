using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Vehicle_Controller : ActionBubble_Interactable, ISaveLoadable, IRestrictable
{
    [Space(20)]
    [SerializeField] private VehicleMovement_Controller _movement;
    public VehicleMovement_Controller movement => _movement;

    [SerializeField] private VehicleMenu_Controller _menu;
    public VehicleMenu_Controller menu => _menu;

    [SerializeField] private LocationMenu_Controller _locationMenu;
    public LocationMenu_Controller locationMenu => _locationMenu;
    
    [SerializeField] private Custom_PositionClaimer _positionClaimer;
    public Custom_PositionClaimer positionClaimer => _positionClaimer;
    
    [Space(20)]
    [SerializeField] private GameObject _spritesFile;
    public GameObject spritesFile => _spritesFile;

    [SerializeField] private SpriteRenderer _interactArea;
    public SpriteRenderer interactArea => _interactArea;
    
    [SerializeField] private SpriteRenderer _restrictedArea;

    [Space(20)]
    [SerializeField] private Transform _transparencyPoint;

    [SerializeField] private Transform _stationSpawnPoint;
    public Transform stationSpawnPoint => _stationSpawnPoint;

    [SerializeField] private Transform _driverSeatPoint;
    public Transform driverSeatPoint => _driverSeatPoint;

    [Space(60)] 
    [SerializeField] private VideoGuide_Trigger _guideTrigger;
    
    
    private bool _transparencyLocked;


    // UnityEngine
    private void Awake()
    {
        Main_Controller.instance.Track_CurrentVehicle(this);
    }

    private new void Start()
    {
        base.Start();
        _interactArea.gameObject.SetActive(false);
        
        // set _interactArea as restricted area for NPC free roam
        Main_Controller.instance.currentLocation.restrictAreaDatas.Add(new(_restrictedArea, gameObject));

        // subscription
        detection.ExitEvent += Transparency_Update;
        OnAction1 += Open_VehicleMenu;
        OnInteract += _guideTrigger.Trigger_CurrentGuide;
    }

    private void OnDestroy()
    {
        // subscription
        detection.ExitEvent += Transparency_Update;
        OnAction1 -= Open_VehicleMenu;
        OnInteract -= _guideTrigger.Trigger_CurrentGuide;
    }

    private void Update()
    {
        Transparency_Update();
    }


    // ISaveLoadable
    public void Save_Data()
    {
        ES3.Save("Vehicle_Controller/_interactArea.size", _interactArea.size);
    }

    public void Load_Data()
    {
        _interactArea.size = ES3.Load("Vehicle_Controller/_interactArea.size", _interactArea.size);
    }
    
    
    // IRestrictable
    public bool IsRestricted()
    {
        Main_Controller main = Main_Controller.instance;
        
        if (_restrictedArea.bounds.Contains(main.Player().transform.position) == false) return false;
        
        List<Station_Controller> allCurrentStations = main.currentStations;
        int count = 0;
        
        for (int i = 0; i < allCurrentStations.Count; i++)
        {
            if (_restrictedArea.bounds.Contains(allCurrentStations[i].transform.position) == false) continue;
            if (allCurrentStations[i].Food_Icon() == null) continue;
            if (allCurrentStations[i].movement == null || allCurrentStations[i].movement.enabled) continue;
            
            count++;
            if (count >= 2) return true;
        }
        return false;
    }
    

    // Vehicle Sprite Control
    private void Transparency_Update()
    {
        if (_transparencyLocked || detection.player == null)
        {
            LeanTween.alpha(_spritesFile, 1f, 0f);
            return;
        }

        if (detection.player.transform.position.y > _transparencyPoint.position.y)
        {
            LeanTween.alpha(_spritesFile, 0.3f, 0f);
            return;
        }

        LeanTween.alpha(_spritesFile, 1f, 0f);
    }

    public void Toggle_TransparencyLock(bool toggle)
    {
        _transparencyLocked = toggle;

        if (toggle == false) return;
        LeanTween.alpha(_spritesFile, 1f, 0f);
    }


    // Interact Area Control
    public Vector2 Random_InteractPoint()
    {
        Location_Controller location = Main_Controller.instance.currentLocation;
        Vector2 pointPos = Utility.Random_BoundPoint(_interactArea.bounds);

        while (location.Is_OuterSpawnPoint(pointPos))
        {
            pointPos = Utility.Random_BoundPoint(_interactArea.bounds);
        }
        
        return pointPos;
    }

    public bool Is_InteractArea(Vector2 checkPosition)
    {
        Bounds bounds = _interactArea.bounds;
        return bounds.Contains(checkPosition);
    }
    
    public void Update_InteractArea_Range(Vector2 updateValue)
    {
        _interactArea.size = updateValue;
    }


    // Menu Toggles
    private void Open_VehicleMenu()
    {
        _menu.VehicleMenu_Toggle(true);
    }

    public void Open_LocationMenu()
    {
        _locationMenu.Toggle_Menu(true);
    }
}
