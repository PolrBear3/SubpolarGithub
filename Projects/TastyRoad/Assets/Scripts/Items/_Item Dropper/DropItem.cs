using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour, IInteractable
{
    private SpriteRenderer _sr;
    public SpriteRenderer sr => _sr;

    private Main_Controller _main;

    [SerializeField] private Detection_Controller _detection;
    [SerializeField] private CoinLauncher _launcher;

    private ItemSlot_Data _itemData;


    // UnityEngine
    private void Awake()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();
        _main = GameObject.FindGameObjectWithTag("MainController").GetComponent<Main_Controller>();
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


    //
    private void Pickup()
    {
        if (_itemData == null) return;

        Sprite launchSprite;

        // food
        if (_itemData.currentFood != null)
        {
            FoodMenu_Controller foodMenu = _main.currentVehicle.menu.foodMenu;
            foodMenu.Add_FoodItem(_itemData.currentFood, _itemData.currentAmount);

            launchSprite = _itemData.currentFood.sprite;
        }
        // station
        else
        {
            StationMenu_Controller stationMenu = _main.currentVehicle.menu.stationMenu;
            stationMenu.Add_StationItem(_itemData.currentStation, _itemData.currentAmount);

            launchSprite = _itemData.currentStation.sprite;
        }

        // launch
        _launcher.Parabola_CoinLaunch(launchSprite, _main.Player().transform.position);

        Destroy(gameObject, 0.1f);
    }
}
