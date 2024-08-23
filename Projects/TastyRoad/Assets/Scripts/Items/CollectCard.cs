using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectCard : MonoBehaviour
{
    private SpriteRenderer _sr;
    public SpriteRenderer sr => _sr;

    [Header("")]
    [SerializeField] private ActionBubble_Interactable _interactable;

    [Header("")]
    [SerializeField] private CoinLauncher _launcher;

    [SerializeField] private Sprite _launchSprite;
    public Sprite launchSprite => _launchSprite;

    [Header("")]
    [SerializeField] private Station_ScrObj[] _bluePrintStations;

    public delegate void Event();
    public List<Event> _allInteractions = new();

    private Event _setInteraction;


    // UnityEngine
    private void Awake()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        Set_RandomInteraction();

        _interactable.Action1Event += Invoke_Interaction;
    }

    private void OnDestroy()
    {
        _interactable.Action1Event -= Invoke_Interaction;
    }


    // Event Interaction
    private void Set_RandomInteraction()
    {
        // add all interact Functions here //
        _allInteractions.Add(FoodIngredient_toArchive);
        _allInteractions.Add(StationBluePrint_toArchive);

        // set random interaction from event _allInteractions
        int randIndex = Random.Range(0, _allInteractions.Count);
        _setInteraction = _allInteractions[randIndex];
    }

    private void Invoke_Interaction()
    {
        _interactable.LockInteract(true);

        _launcher.Parabola_CoinLaunch(_launchSprite, _interactable.detection.player.transform.position);
        _setInteraction?.Invoke();

        Destroy(gameObject, 0.1f);
    }


    // Functions
    private void FoodIngredient_toArchive()
    {
        Food_ScrObj randFood = _interactable.mainController.dataController.CookedFood();
        ArchiveMenu_Controller menu = _interactable.mainController.currentVehicle.menu.archiveMenu;

        if (menu.Ingredient_Unlocked(randFood) == false)
        {
            // add food and lock bookmarking
            menu.AddFood(randFood).Toggle_Lock(true);
        }

        // unlock only ingredient
        menu.UnLock_Ingredient(randFood);

        // dialog
        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();
        dialog.Update_Dialog(new DialogData(dialog.defaultData.icon, dialog.datas[0].info));
    }

    private void StationBluePrint_toArchive()
    {
        Station_ScrObj randStation = _bluePrintStations[Random.Range(0, _bluePrintStations.Length)];
        StationMenu_Controller menu = _interactable.mainController.currentVehicle.menu.stationMenu;

        if (menu.controller.slotsController.Slots_Full() == true)
        {
            // dialog
            return;
        }

        // add station blueprint
        menu.Add_StationItem(randStation, 1).Toggle_Lock(true);

        // dialog
        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();
        dialog.Update_Dialog(new DialogData(dialog.defaultData.icon, dialog.datas[1].info));
    }
}
