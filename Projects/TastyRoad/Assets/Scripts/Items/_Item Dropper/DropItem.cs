using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour, IInteractable
{
    private SpriteRenderer _sr;

    private Main_Controller _main;

    [Header("")]
    [SerializeField] private Detection_Controller _detection;
    [SerializeField] private CoinLauncher _launcher;

    private ItemSlot_Data _itemData;

    [Header("")]
    [SerializeField][Range(0, 100)] private int _destroyTikCount;
    private int _currentTikCount;


    // UnityEngine
    private void Awake()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();
        _main = GameObject.FindGameObjectWithTag("MainController").GetComponent<Main_Controller>();
    }

    private void Start()
    {
        GlobalTime_Controller.TimeTik_Update += Activate_DestroyTimeTik;
    }

    private void OnDestroy()
    {
        GlobalTime_Controller.TimeTik_Update -= Activate_DestroyTimeTik;
    }


    // IInteractable
    public void Interact()
    {
        Pickup();
    }

    public void UnInteract()
    {

    }


    // Set
    public void Set_ItemData(ItemSlot_Data setData)
    {
        _itemData = setData;
    }


    // Functions
    private void Pickup()
    {
        if (_itemData == null) return;

        VehicleMenu_Controller menu = _main.currentVehicle.menu;
        ItemSlots_Controller slotsController = menu.slotsController;

        Sprite launchSprite;
        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();

        // food
        if (_itemData.currentFood != null)
        {
            FoodMenu_Controller foodMenu = menu.foodMenu;

            // food menu empty slots available check
            if (slotsController.Empty_SlotData(foodMenu.currentDatas) == null)
            {
                dialog.Update_Dialog(0);
                return;
            }

            foodMenu.Add_FoodItem(_itemData.currentFood, _itemData.currentAmount);
            launchSprite = _itemData.currentFood.sprite;
        }
        // station
        else
        {
            StationMenu_Controller stationMenu = menu.stationMenu;

            // station menu empty slots available check
            if (slotsController.Empty_SlotData(stationMenu.currentDatas) == null)
            {
                dialog.Update_Dialog(1);
                return;
            }

            stationMenu.Add_StationItem(_itemData.currentStation, _itemData.currentAmount);
            launchSprite = _itemData.currentStation.sprite;
        }

        // launch
        _launcher.Parabola_CoinLaunch(launchSprite, _main.Player().transform.position);

        Destroy(gameObject, 0.1f);
    }

    private void Activate_DestroyTimeTik()
    {
        float alphaStepSize = 100 / _destroyTikCount * 0.01f;

        _currentTikCount++;
        Main_Controller.Change_SpriteAlpha(_sr, _sr.color.a - alphaStepSize);

        if (_currentTikCount < _destroyTikCount) return;
        Destroy(gameObject, 0.1f);
    }
}
