using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectCard : MonoBehaviour, IInteractable
{
    private SpriteRenderer _sr;
    public SpriteRenderer sr => _sr;

    private Main_Controller _mainController;

    [Header("")]
    [SerializeField] private Detection_Controller _detection;
    [SerializeField] private CoinLauncher _launcher;

    [Header("")]
    [SerializeField] private Sprite _launchSprite;
    public Sprite launchSprite => _launchSprite;

    [Header("")]
    [SerializeField][Range(0, 100)] private int _destroyTikCount;
    private int _currentTikCount;


    private ItemSlot_Data _collectData;

    private List<Action> Pickup_Actions = new();
    private Action OnPickup;


    // UnityEngine
    private void Awake()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();
        _mainController = GameObject.FindGameObjectWithTag("MainController").GetComponent<Main_Controller>();
    }

    private void Start()
    {
        Set_RandomPickup();

        GlobalTime_Controller.TimeTik_Update += Activate_DestroyTimeTik;
    }

    private void OnDestroy()
    {
        GlobalTime_Controller.TimeTik_Update -= Activate_DestroyTimeTik;
    }


    // IInteractable
    public void Interact()
    {
        OnPickup?.Invoke();
    }

    public void Hold_Interact()
    {

    }

    public void UnInteract()
    {

    }


    // Event Interaction
    private void Set_RandomPickup()
    {
        // add all pickup Functions
        Pickup_Actions.Add(FoodIngredient_toArchive);
        Pickup_Actions.Add(StationBluePrint_toArchive);

        // set random interaction from event _allInteractions
        int randIndex = UnityEngine.Random.Range(0, Pickup_Actions.Count);
        OnPickup = Pickup_Actions[randIndex];
    }


    // Food Ingredient
    public void Set_FoodIngredient(Food_ScrObj food)
    {
        _collectData = new(new FoodData(food));
    }

    public void SetLocation_FoodIngredient(bool set)
    {
        if (set == false) return;

        Location_Controller currentLocation = _mainController.currentLocation;

        Set_FoodIngredient(currentLocation.data.WeightRandom_Food());
    }


    private void FoodIngredient_toArchive()
    {
        ArchiveMenu_Controller menu = _mainController.currentVehicle.menu.archiveMenu;
        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();

        SetLocation_FoodIngredient(_collectData == null);

        Food_ScrObj archiveFood = _collectData.currentFood;
        ItemSlot_Data foodData = menu.Archived_FoodData(archiveFood);

        bool foodUnlocked = foodData != null && foodData.isLocked == false;

        menu.Toggle_DataLock(menu.Archive_Food(archiveFood), foodUnlocked == false);
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

        Location_Controller currentLocation = _mainController.currentLocation;

        Set_Blueprint(currentLocation.data.WeightRandom_Station());
    }


    private void StationBluePrint_toArchive()
    {
        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();
        StationMenu_Controller menu = _mainController.currentVehicle.menu.stationMenu;

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


    // Functions
    private void Activate_DestroyTimeTik()
    {
        float alphaStepSize = 100 / _destroyTikCount * 0.01f;

        _currentTikCount++;
        Main_Controller.Change_SpriteAlpha(_sr, _sr.color.a - alphaStepSize);

        if (_currentTikCount < _destroyTikCount) return;
        Destroy(gameObject, 0.1f);
    }
}
