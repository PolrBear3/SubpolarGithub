using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodBox : MonoBehaviour, IInteractable
{
    private Station_Controller _controller;



    // UnityEngine
    private void Awake()
    {
        _controller = gameObject.GetComponent<Station_Controller>();
    }

    private void Start()
    {
        _controller.Food_Icon().AmountBar_Transparency(true);
    }



    // OnTrigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Player_Controller player)) return;

        _controller.Food_Icon().AmountBar_Transparency(false);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Player_Controller player)) return;

        _controller.Food_Icon().AmountBar_Transparency(true);
    }



    // IInteractable
    public void Interact()
    {
        Give_Food();

        if (_controller.Food_Icon().currentFoodData.currentAmount > 0) return;

        Empty_Destroy();
    }

    public void UnInteract()
    {

    }



    // Give Food to Player
    private void Give_Food()
    {
        FoodData_Controller playerIcon = _controller.detection.player.foodIcon;

        if (playerIcon.hasFood) return;

        FoodData_Controller thisIcon = _controller.Food_Icon();

        // give player food
        playerIcon.Assign_Food(thisIcon.currentFoodData.foodScrObj);
        playerIcon.Assign_State(thisIcon.currentFoodData.stateData);

        // decrease one amount
        thisIcon.Update_Amount(-1);
    }

    private void Empty_Destroy()
    {
        _controller.mainController.UnTrack_CurrentStation(_controller);
        Destroy(gameObject);
    }
}
