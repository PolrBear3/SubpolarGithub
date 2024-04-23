using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Controller : MonoBehaviour, ISaveLoadable
{
    private PlayerInput _playerInput;

    [HideInInspector] public Main_Controller mainController;

    [HideInInspector] public Detection_Controller detectionController;
    [HideInInspector] public BasicAnimation_Controller animationController;

    [SerializeField] FoodData_Controller _foodIcon;
    public FoodData_Controller foodIcon => _foodIcon;

    [SerializeField] private CoinLauncher _coinLauncher;
    public CoinLauncher coinLauncher => _coinLauncher;

    [HideInInspector] public Player_Movement movement;
    [HideInInspector] public Player_Interaction interaction;



    // UnityEngine
    private void Awake()
    {
        mainController = FindObjectOfType<Main_Controller>();
        mainController.Track_CurrentCharacter(gameObject);

        if (gameObject.TryGetComponent(out PlayerInput playerInput)) { _playerInput = playerInput; }

        if (gameObject.TryGetComponent(out Detection_Controller detectionController)) { this.detectionController = detectionController; }
        if (gameObject.TryGetComponent(out BasicAnimation_Controller animationController)) { this.animationController = animationController; }

        if (gameObject.TryGetComponent(out Player_Movement movement)) { this.movement = movement; }
    }



    // ISaveLoadable
    public void Save_Data()
    {
        if (_foodIcon.currentData == null) return;
        ES3.Save("Player_Controller/_foodIcon.currentData", _foodIcon.currentData);
    }

    public void Load_Data()
    {
        if (ES3.KeyExists("Player_Controller/_foodIcon.currentData") == false) return;
        _foodIcon.Set_CurrentData(ES3.Load<FoodData>("Player_Controller/_foodIcon.currentData"));
    }



    //
    public PlayerInput Player_Input()
    {
        return _playerInput;
    }
}