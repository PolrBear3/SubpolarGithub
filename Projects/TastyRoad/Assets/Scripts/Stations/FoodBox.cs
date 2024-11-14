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
        _controller.detection.EnterEvent += Toggle_AmountBar;
        _controller.detection.ExitEvent += Toggle_AmountBar;
    }

    private void OnDestroy()
    {
        _controller.detection.EnterEvent -= Toggle_AmountBar;
        _controller.detection.ExitEvent -= Toggle_AmountBar;
    }


    // IInteractable
    public void Interact()
    {
        Transfer_Food();
        Empty_Destroy();
    }

    public void Hold_Interact()
    {
        Transfer_All();
        Empty_Destroy();
    }

    public void UnInteract()
    {

    }


    // Functions
    private void Toggle_AmountBar()
    {
        _controller.Food_Icon().Toggle_AmountBar(_controller.detection.player != null);
    }


    private void Transfer_Food()
    {
        FoodData_Controller playerIcon = _controller.detection.player.foodIcon;

        if (playerIcon.SubDataCount_Maxed()) return;

        FoodData_Controller stationIcon = _controller.Food_Icon();

        // transfer to player
        playerIcon.Set_CurrentData(stationIcon.currentData);

        playerIcon.Show_Icon();
        playerIcon.Show_Condition();
        playerIcon.Toggle_SubDataBar(true);

        // decrease one amount
        stationIcon.currentData.Update_Amount(-1);
        stationIcon.Show_AmountBar();

        // sound
        Audio_Controller.instance.Play_OneShot("FoodInteract_swap", transform.position);
    }

    private void Transfer_All()
    {
        FoodData_Controller playerIcon = _controller.detection.player.foodIcon;

        if (playerIcon.SubDataCount_Maxed()) return;

        FoodData_Controller stationIcon = _controller.Food_Icon();

        int transferCount = 0;

        for (int i = 0; i < stationIcon.currentData.currentAmount; i++)
        {
            playerIcon.Set_CurrentData(stationIcon.currentData);
            transferCount++;

            if (playerIcon.SubDataCount_Maxed()) break;
        }

        playerIcon.Show_Icon();
        playerIcon.Show_Condition();
        playerIcon.Toggle_SubDataBar(true);

        stationIcon.currentData.Update_Amount(-transferCount);
        stationIcon.Show_AmountBar();
    }


    private void Empty_Destroy()
    {
        if (_controller.Food_Icon().currentData.currentAmount > 0) return;

        // clear current food data
        _controller.Food_Icon().Set_CurrentData(null);

        // station control
        Main_Controller main = _controller.mainController;

        main.UnTrack_CurrentStation(_controller);
        main.UnClaim_Position(transform.position);

        // sound !

        Destroy(gameObject);
    }
}
