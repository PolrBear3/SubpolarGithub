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
        _controller.Food_Icon().Show_AmountBar_Duration();
    }



    // OnTrigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Player_Controller player)) return;

        _controller.Food_Icon().ShowAmountBar_LockToggle(false);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Player_Controller player)) return;

        _controller.Food_Icon().ShowAmountBar_LockToggle(true);
    }



    // IInteractable
    public void Interact()
    {
        Give_Food();

        if (_controller.Food_Icon().currentData.currentAmount <= 0)
        {
            Empty_Destroy();
            return;
        }

        _controller.Food_Icon().Show_AmountBar();
    }

    public void UnInteract()
    {

    }



    // Give Food to Player
    private void Give_Food()
    {
        FoodData_Controller playerIcon = _controller.detection.player.foodIcon;

        if (playerIcon.hasFood) return;

        // give player food
        playerIcon.Set_CurrentData(new FoodData(_controller.Food_Icon().currentData.foodScrObj));
        playerIcon.Show_Icon();

        // decrease one amount
        _controller.Food_Icon().currentData.Update_Amount(-1);

        // if has condition, transfer condition as well
        if (_controller.Food_Icon().currentData.conditionDatas.Count <= 0) return;

        playerIcon.currentData.Set_Condition(_controller.Food_Icon().currentData.conditionDatas);
        playerIcon.Show_Condition();
    }

    private void Empty_Destroy()
    {
        // clear current food data
        _controller.Food_Icon().Set_CurrentData(null);

        // station control
        Main_Controller main = _controller.mainController;

        main.UnTrack_CurrentStation(_controller);
        main.UnClaim_Position(transform.position);

        Destroy(gameObject);
    }
}
