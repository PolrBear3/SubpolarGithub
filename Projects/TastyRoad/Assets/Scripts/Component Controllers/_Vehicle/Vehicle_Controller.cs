using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Vehicle_Controller : ActionBubble_Interactable, ISaveLoadable
{
    [Header("")]
    [SerializeField] private VehicleMenu_Controller _menu;
    public VehicleMenu_Controller menu => _menu;

    [SerializeField] private VehicleMovement_Controller _movement;

    [Header("")]
    [SerializeField] private Custom_PositionClaimer _positionClaimer;
    public Custom_PositionClaimer positionClaimer => _positionClaimer;

    [Header("")]
    [SerializeField] private GameObject _spritesFile;
    public GameObject spritesFile => _spritesFile;

    [Header("")]
    [SerializeField] private SpriteRenderer _interactArea;
    public SpriteRenderer interactArea => _interactArea;

    [SerializeField] private Vector2[] _areaSizes;

    [Header("")]
    [SerializeField] private Transform _transparencyPoint;

    private bool _transparencyLocked;

    [SerializeField] private Transform _stationSpawnPoint;
    public Transform stationSpawnPoint => _stationSpawnPoint;

    [SerializeField] private Transform _driverSeatPoint;
    public Transform driverSeatPoint => _driverSeatPoint;


    // UnityEngine
    private new void Awake()
    {
        base.Awake();
        mainController.Track_CurrentVehicle(this);
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


    // Interact Area Control
    public bool Is_InteractArea(Vector2 checkPosition)
    {
        Bounds bounds = _interactArea.bounds;
        return bounds.Contains(checkPosition);
    }


    public int InteractAreaSize_IndexNum(Vector2 areaSize)
    {
        for (int i = 0; i < _areaSizes.Length; i++)
        {
            if (areaSize != _areaSizes[i]) continue;
            return i;
        }

        Debug.Log("Interact area size index num not found!");
        return 0;
    }

    public void Update_InteractArea(int sizeIndexNum)
    {
        sizeIndexNum = Mathf.Clamp(sizeIndexNum, 0, _areaSizes.Length - 1);
        Vector2 updateSize = _areaSizes[sizeIndexNum];

        _interactArea.size = updateSize;
    }


    // Action Options
    private void Open_VehicleMenu()
    {
        detection.player.Player_Input().enabled = false;

        _menu.VehicleMenu_Toggle(true);
    }
}
