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


    private Station_ScrObj _blueprintPickup;
    // private Food_ScrObj _ingredientPickup;

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
    private void FoodIngredient_toArchive()
    {
        Food_ScrObj randFood = _mainController.dataController.CookedFood();
        ArchiveMenu_Controller menu = _mainController.currentVehicle.menu.archiveMenu;

        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();

        // available slots check
        if (menu.controller.slotsController.Empty_SlotData(menu.currentDatas) == null)
        {
            dialog.Update_Dialog(new DialogData(dialog.defaultData.icon, dialog.datas[2].info));
            return;
        }

        if (menu.Food_Archived(randFood) == false)
        {
            // add food and lock bookmarking
            menu.Archive_Food(randFood).isLocked = true;

            // unlcok food ingredient
            menu.Unlock_FoodIngredient(randFood);
        }

        // dialog
        dialog.Update_Dialog(new DialogData(dialog.defaultData.icon, dialog.datas[0].info));

        // pickup
        _launcher.Parabola_CoinLaunch(_launchSprite, _detection.player.transform.position);
        Destroy(gameObject, 0.1f);
    }


    // Station Bluepirnt
    public void Set_Blueprint(Station_ScrObj station)
    {
        _blueprintPickup = station;
    }

    /// <summary>
    /// Set station to current location weight random data
    /// </summary>
    public void Set_Blueprint()
    {
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

        if (_blueprintPickup == null)
        {
            Set_Blueprint();
        }

        // add station blueprint
        menu.Add_StationItem(_blueprintPickup, 1).isLocked = true;

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
