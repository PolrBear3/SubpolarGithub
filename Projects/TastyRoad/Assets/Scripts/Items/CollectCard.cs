using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectCard : MonoBehaviour
{
    [SerializeField] private ActionBubble_Interactable _interactable;

    public delegate void Event();
    public List<Event> _allInteractions = new();


    // UnityEngine
    private void Start()
    {
        Set_Interactions();

        _interactable.Action1Event += Invoke_RandomInteraction;
    }

    private void OnDestroy()
    {
        _interactable.Action1Event -= Invoke_RandomInteraction;
    }


    // Event Interaction
    private void Set_Interactions()
    {
        // add all interact Functions here //
        _allInteractions.Add(FoodIngredient_toArchive);
    }

    private void Invoke_RandomInteraction()
    {
        // check if there are interactions
        if (_allInteractions.Count <= 0) return;

        int randIndex = Random.Range(0, _allInteractions.Count);

        _allInteractions[randIndex]?.Invoke();
    }


    // Functions
    private void FoodIngredient_toArchive()
    {
        Food_ScrObj randFood = _interactable.mainController.dataController.CookedFood();

        ArchiveMenu_Controller menu = _interactable.mainController.currentVehicle.menu.archiveMenu;
        menu.AddFood(randFood);

        // unlock only ingredient
        menu.UnLock_Ingredient(randFood);

        // remove
        Destroy(gameObject);
    }
}
