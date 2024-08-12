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

    [Header("")]
    [SerializeField] private Sprite _launchSprite;
    public Sprite launchSprite => _launchSprite;

    private Food_ScrObj _foodItem;
    private int _itemAmount;


    // UnityEngine
    private void Awake()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();
        _main = GameObject.FindGameObjectWithTag("MainController").GetComponent<Main_Controller>();
    }


    // IInteractable
    public void Interact()
    {
        if (_foodItem == null)
        {
            Random_Pickup();
            return;
        }

        Pickup();
    }

    public void UnInteract()
    {
        
    }


    // Set
    public void Set_FoodItem(Food_ScrObj setFood, int setAmount)
    {
        _foodItem = setFood;
        _itemAmount = setAmount;
    }


    //
    private void Random_Pickup()
    {
        _launcher.Parabola_CoinLaunch(_launchSprite, _main.Player().transform.position);
        Destroy(gameObject, 0.1f);
    }

    private void Pickup()
    {
        FoodMenu_Controller foodMenu = _main.currentVehicle.menu.foodMenu;
        foodMenu.Add_FoodItem(_foodItem, _itemAmount);

        _launcher.Parabola_CoinLaunch(_launchSprite, _main.Player().transform.position);
        Destroy(gameObject, 0.1f);
    }
}
