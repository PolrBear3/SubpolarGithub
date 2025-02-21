using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectCard : MonoBehaviour
{
    private SpriteRenderer _sr;
    public SpriteRenderer sr => _sr;


    [Header("")]
    [SerializeField] private Detection_Controller _detection;
    [SerializeField] private IInteractable_Controller _interactable;
    [SerializeField] private CoinLauncher _launcher;

    [Header("")]
    [SerializeField] private Sprite _launchSprite;
    public Sprite launchSprite => _launchSprite;

    [Header("")]
    [SerializeField][Range(0, 100)] private int _destroyTikCount;
    [SerializeField][Range(0, 1)] private float _transparencyStep;
    private int _currentTikCount;


    private ItemSlot_Data _collectData;

    private List<Action> Pickup_Actions = new();


    // UnityEngine
    private void Awake()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        GlobalTime_Controller.instance.OnTimeTik += Activate_DestroyTimeTik;

        _interactable.OnInteract += Pickup;
    }

    private void OnDestroy()
    {
        GlobalTime_Controller.instance.OnTimeTik -= Activate_DestroyTimeTik;

        _interactable.OnInteract -= Pickup;
    }


    // Pickup Interaction
    public void Add_RandomPickup(Action pickupAction)
    {
        Pickup_Actions.Add(pickupAction);
    }

    private void Add_AllPickups()
    {
        // add all pickup Functions
        Pickup_Actions.Add(FoodIngredient_toArchive);
        Pickup_Actions.Add(StationBluePrint_toArchive);
    }


    public void Pickup()
    {
        if (Pickup_Actions == null || Pickup_Actions.Count <= 0) Add_AllPickups();

        int randIndex = UnityEngine.Random.Range(0, Pickup_Actions.Count);
        Pickup_Actions[randIndex]?.Invoke();
    }


    // Food Ingredient
    public void Set_FoodIngredient(Food_ScrObj food)
    {
        _collectData = new(new FoodData(food));
    }

    public void SetLocation_FoodIngredient(bool set)
    {
        if (set == false) return;

        Location_Controller currentLocation = Main_Controller.instance.currentLocation;
        Set_FoodIngredient(currentLocation.data.WeightRandom_Food());
    }


    private void FoodIngredient_toArchive()
    {
        ArchiveMenu_Controller menu = Main_Controller.instance.currentVehicle.menu.archiveMenu;
        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();

        SetLocation_FoodIngredient(_collectData == null);

        Food_ScrObj archiveFood = _collectData.currentFood;

        ItemSlot_Data existingData = menu.Archived_FoodData(archiveFood);
        ItemSlot_Data newData = menu.Archive_Food(archiveFood);

        bool foodUnlocked = existingData != null && existingData.isLocked == false;

        menu.Toggle_DataLock(newData, foodUnlocked == false);
        menu.Unlock_FoodIngredient(archiveFood);

        // dialog
        dialog.Update_Dialog(new DialogData(dialog.defaultData.icon, dialog.datas[0].info));

        // pickup animation
        _launcher.Parabola_CoinLaunch(_launchSprite, _detection.player.transform.position);
        Destroy(gameObject, 0.1f);
    }


    // Station Bluepirnt
    public void Set_Blueprint(Station_ScrObj station)
    {
        _collectData = new(new StationData(station));
    }

    public void SetLocation_Blueprint(bool set)
    {
        if (set == false) return;

        Location_Controller currentLocation = Main_Controller.instance.currentLocation;
        Set_Blueprint(currentLocation.data.WeightRandom_Station());
    }


    public void StationBluePrint_toArchive()
    {
        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();
        StationMenu_Controller menu = Main_Controller.instance.currentVehicle.menu.stationMenu;

        // available slots check
        if (menu.controller.slotsController.Empty_SlotData(menu.currentDatas) == null)
        {
            dialog.Update_Dialog(new DialogData(dialog.defaultData.icon, dialog.datas[3].info));
            return;
        }

        SetLocation_Blueprint(_collectData == null);

        // add station blueprint
        Station_ScrObj addStation = _collectData.currentStation;
        menu.Toggle_DataLock(menu.Add_StationItem(addStation, 1), true);

        // dialog
        dialog.Update_Dialog(new DialogData(dialog.defaultData.icon, dialog.datas[1].info));

        // pickup
        _launcher.Parabola_CoinLaunch(_launchSprite, _detection.player.transform.position);
        Destroy(gameObject, 0.1f);
    }


    // Time Tik Destroy
    private void Activate_DestroyTimeTik()
    {
        _currentTikCount++;
        Main_Controller.instance.Change_SpriteAlpha(_sr, _sr.color.a - _transparencyStep);

        if (_currentTikCount < _destroyTikCount) return;
        Destroy(gameObject, 0.1f);
    }
}
