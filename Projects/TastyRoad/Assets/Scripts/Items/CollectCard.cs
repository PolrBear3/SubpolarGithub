using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectCard : MonoBehaviour
{
    private SpriteRenderer _sr;
    public SpriteRenderer sr => _sr;

    private Main_Controller _mainController;

    [Header("")]
    [SerializeField] private Detection_Controller _detection;


    [Header("")]
    [SerializeField] private CoinLauncher _launcher;

    [SerializeField] private Sprite _launchSprite;
    public Sprite launchSprite => _launchSprite;

    [Header("")]
    [SerializeField] private Station_ScrObj[] _bluePrintStations;

    [Header("")]
    [SerializeField][Range(0, 100)] private int _destroyTikCount;
    private int _currentTikCount;


    public delegate bool EventHandler();

    private List<EventHandler> OnPickups = new();
    private event EventHandler OnPickup;


    // UnityEngine
    private void Awake()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();
        _mainController = GameObject.FindGameObjectWithTag("MainController").GetComponent<Main_Controller>();
    }

    private void Start()
    {
        Set_RandomPickup();

        _detection.EnterEvent += Pickup;
        GlobalTime_Controller.TimeTik_Update += Activate_DestroyTimeTik;
    }

    private void OnDestroy()
    {
        _detection.EnterEvent -= Pickup;
        GlobalTime_Controller.TimeTik_Update -= Activate_DestroyTimeTik;
    }


    // Event Interaction
    private void Set_RandomPickup()
    {
        // add all pickup Functions here !
        OnPickups.Add(FoodIngredient_toArchive);
        OnPickups.Add(StationBluePrint_toArchive);

        // set random interaction from event _allInteractions
        int randIndex = Random.Range(0, OnPickups.Count);
        OnPickup = OnPickups[randIndex];
    }

    private void Pickup()
    {
        if (OnPickup?.Invoke() == false) return;

        _launcher.Parabola_CoinLaunch(_launchSprite, _detection.player.transform.position);
        Destroy(gameObject, 0.1f);
    }


    // On Pickups
    private bool FoodIngredient_toArchive()
    {
        Food_ScrObj randFood = _mainController.dataController.CookedFood();
        ArchiveMenu_Controller menu = _mainController.currentVehicle.menu.archiveMenu;

        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();

        // available slots check
        if (menu.controller.slotsController.Empty_SlotData(menu.currentDatas) == null)
        {
            // dialog
            dialog.Update_Dialog(new DialogData(dialog.defaultData.icon, dialog.datas[2].info));

            return false;
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

        return true;
    }

    private bool StationBluePrint_toArchive()
    {
        Station_ScrObj randStation = _bluePrintStations[Random.Range(0, _bluePrintStations.Length)];
        StationMenu_Controller menu = _mainController.currentVehicle.menu.stationMenu;

        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();

        // available slots check
        if (menu.controller.slotsController.Empty_SlotData(menu.currentDatas) == null)
        {
            // dialog
            dialog.Update_Dialog(new DialogData(dialog.defaultData.icon, dialog.datas[3].info));

            return false;
        }

        // add station blueprint
        menu.Add_StationItem(randStation, 1).isLocked = true;

        // dialog
        dialog.Update_Dialog(new DialogData(dialog.defaultData.icon, dialog.datas[1].info));

        return true;
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
