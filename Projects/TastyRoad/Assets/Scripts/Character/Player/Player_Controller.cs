using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Controller : MonoBehaviour
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

    private void Start()
    {
        Main_Controller.TestButton1Event += Set_Food;
    }



    // ISaveLoadable
    /*
    public void Save_Data()
    {
        FoodData playerFoodData = new(foodIcon.currentData);
        ES3.Save("Player_Controller/foodIcon.currentFoodData", playerFoodData);
    }

    public void Load_Data()
    {
        foodIcon.Assign_Food(ES3.Load("Player_Controller/foodIcon.currentFoodData", foodIcon.currentData));
        foodIcon.Load_FoodData();

        // foodIcon.stateBoxController.Update_StateBoxes();
    }
    */



    //
    public PlayerInput Player_Input()
    {
        return _playerInput;
    }



    //
    private void Set_Food()
    {
        Food_ScrObj apple = mainController.dataController.rawFoods[0];

        _foodIcon.Set_CurrentData(new FoodData(apple));
        _foodIcon.Show_Icon();
    }
}