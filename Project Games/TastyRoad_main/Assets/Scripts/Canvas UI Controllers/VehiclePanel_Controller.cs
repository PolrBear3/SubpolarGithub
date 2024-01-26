using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VehiclePanel_Controller : MonoBehaviour
{
    [HideInInspector] public FoodMenu_Controller foodMenu;
    [HideInInspector] public ArchiveMenu_Controller archiveMenu;

    [HideInInspector] public List<VechiclePanel_ItemBox> itemBoxes = new();
    [HideInInspector] public VechiclePanel_ItemBox currentItemBox;

    [Header("Insert Vehicle Prefab")]
    public Vehicle_Controller vehicleController;

    [Header("Menu Control")]
    [SerializeField] private List<GameObject> _menus = new();
    [SerializeField] private List<GameObject> _menuIcons = new();

    private int _currentMenuNum;
    public int currentMenuNum => _currentMenuNum;

    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out FoodMenu_Controller foodMenu)) { this.foodMenu = foodMenu; }
        if (gameObject.TryGetComponent(out ArchiveMenu_Controller archiveMenu)) { this.archiveMenu = archiveMenu; }
    }
    private void Start()
    {
        Menu_Control(0);

        // test function for demo
        Data_Controller data = vehicleController.mainController.dataController;

        foodMenu.Add_FoodItem(data.RawFood(0), 998);
        foodMenu.Add_FoodItem(data.RawFood(1), 998);
        foodMenu.Add_FoodItem(data.RawFood(2), 998);
        foodMenu.Add_FoodItem(data.RawFood(3), 998);
        foodMenu.Add_FoodItem(data.RawFood(5), 998);
    }

    // InputSystem
    private void OnCursorControl(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();

        CursorDirection_Control(input);
    }

    private void OnOption1()
    {
        Menu_Control(-1);
    }

    private void OnOption2()
    {
        Menu_Control(1);
    }

    private void OnExit()
    {
        if (foodMenu.fridgeTargetMode == true) return;

        vehicleController.VehiclePanel_Toggle(false);

        vehicleController.Player_InputSystem_Toggle(true);
    }

    // Item Box Main Control
    public void Assign_All_BoxNum()
    {
        int numCount = 0;

        for (int i = 0; i < itemBoxes.Count; i++)
        {
            itemBoxes[i].boxNum = numCount;

            numCount++;
        }
    }

    private void CursorDirection_Control(Vector2 inputDireciton)
    {
        if (foodMenu.fridgeTargetMode == true) return;

        float calculatedDirection = inputDireciton.x + -(inputDireciton.y * 5);
        int convertedDirection = (int)calculatedDirection;

        int nextBoxNum = currentItemBox.boxNum + convertedDirection;

        // less than min box num
        if (nextBoxNum < 0)
        {
            nextBoxNum = itemBoxes.Count - 1;
        }

        // more than max box num
        if (nextBoxNum > itemBoxes.Count - 1)
        {
            nextBoxNum = 0;
        }

        currentItemBox.BoxSelect_Toggle(false);

        currentItemBox = itemBoxes[nextBoxNum];

        currentItemBox.BoxSelect_Toggle(true);
    }

    private void Menu_Control(int controlNum)
    {
        if (foodMenu.fridgeTargetMode == true) return;

        _menus[_currentMenuNum].SetActive(false);
        _menuIcons[_currentMenuNum].SetActive(false);

        _currentMenuNum += controlNum;

        if (_currentMenuNum > _menus.Count - 1) _currentMenuNum = 0;
        else if (_currentMenuNum < 0) _currentMenuNum = _menus.Count - 1;

        _menus[_currentMenuNum].SetActive(true);
        _menuIcons[_currentMenuNum].SetActive(true);

        // assign current menu item boxes here
        foodMenu.Assign_ItemBoxes();
        archiveMenu.Assign_ItemBoxes();
        //
    }
}