using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Vehicle_Controller : ActionBubble_Interactable, ISaveLoadable
{
    [Header("")]
    [SerializeField] private VehicleMovement_Controller _movement;
    public VehicleMovement_Controller movement => _movement;

    [SerializeField] private VehicleMenu_Controller _menu;
    public VehicleMenu_Controller menu => _menu;

    [SerializeField] private LocationMenu_Controller _locationMenu;
    public LocationMenu_Controller locationMenu => _locationMenu;


    [SerializeField] private Custom_PositionClaimer _positionClaimer;
    public Custom_PositionClaimer positionClaimer => _positionClaimer;


    [Header("")]
    [SerializeField] private GameObject _spritesFile;
    public GameObject spritesFile => _spritesFile;

    [SerializeField] private SpriteRenderer _interactArea;
    public SpriteRenderer interactArea => _interactArea;


    [Header("")]
    [SerializeField] private Transform _transparencyPoint;

    [SerializeField] private Transform _stationSpawnPoint;
    public Transform stationSpawnPoint => _stationSpawnPoint;

    [SerializeField] private Transform _driverSeatPoint;
    public Transform driverSeatPoint => _driverSeatPoint;

    private bool _transparencyLocked;


    [Header("")]
    [SerializeField] private SpriteRenderer _vehicleBody;
    [SerializeField][Range(0, 10)] private float _materialShineSpeed;

    private Coroutine _materialCoroutine;


    // UnityEngine
    private void Awake()
    {
        Main_Controller.instance.Track_CurrentVehicle(this);
    }

    private new void Start()
    {
        base.Start();

        _interactArea.gameObject.SetActive(false);

        // subscription
        detection.ExitEvent += Transparency_Update;
        OnAction1Input += Open_VehicleMenu;
    }

    private void OnDestroy()
    {
        // subscription
        detection.ExitEvent += Transparency_Update;
        OnAction1Input -= Open_VehicleMenu;
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


    public void SilverShine_VehicleBody()
    {
        if (_materialCoroutine != null) return;

        _materialCoroutine = StartCoroutine(SilverShine_VehicleBody_Coroutine());
    }
    private IEnumerator SilverShine_VehicleBody_Coroutine()
    {
        Material vehicleBody = _vehicleBody.material;

        vehicleBody.SetFloat("_ShineGlow", 0.1f);
        float locationValue = 0;

        while (locationValue < 1)
        {
            locationValue += Time.deltaTime * _materialShineSpeed;
            vehicleBody.SetFloat("_ShineLocation", locationValue);

            yield return null;
        }

        vehicleBody.SetFloat("_ShineGlow", 0f);

        _materialCoroutine = null;
        yield break;
    }


    // Interact Area Control
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
