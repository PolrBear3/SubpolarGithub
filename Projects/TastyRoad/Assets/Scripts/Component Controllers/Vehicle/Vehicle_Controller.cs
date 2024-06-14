using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Vehicle_Controller : ActionBubble_Interactable
{
    [Header("")]
    [SerializeField] private VehicleMenu_Controller _menu;
    public VehicleMenu_Controller menu => _menu;

    [SerializeField] private Vehicle_Customizer _customizer;

    [Header("")]
    [SerializeField] private Transform _transparencyPoint;

    [SerializeField] private Transform _stationSpawnPoint;
    public Transform stationSpawnPoint => _stationSpawnPoint;


    // UnityEngine
    private new void Awake()
    {
        base.Awake();
        mainController.Track_CurrentVehicle(this);
    }

    private new void Start()
    {
        base.Start();

        Action1Event += Open_VehicleMenu;
        Action2Event += Open_WorldMap;
    }

    private void OnDestroy()
    {
        Action1Event -= Open_VehicleMenu;
        Action2Event -= Open_WorldMap;
    }

    private void Update()
    {
        Transparency_Update();
    }


    // Action Options
    private void Open_VehicleMenu()
    {
        detection.player.Player_Input().enabled = false;

        _menu.VehicleMenu_Toggle(true);
    }

    private void Open_WorldMap()
    {
        mainController.worldMap.Map_Toggle(true);
    }


    // OnTrigger
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Player_Controller player)) return;

        UnInteract();
        LeanTween.alpha(_customizer.gameObject, 1f, 0f);
    }



    // Vehicle Sprite Control
    private void Transparency_Update()
    {
        if (detection.player == null) return;

        if (detection.player.transform.position.y > _transparencyPoint.position.y)
        {
            LeanTween.alpha(_customizer.gameObject, 0.3f, 0f);
        }
        else
        {
            LeanTween.alpha(_customizer.gameObject, 1f, 0f);
        }
    }
}
