using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodBox : MonoBehaviour
{
    private Station_Controller _controller;


    // UnityEngine
    private void Awake()
    {
        _controller = gameObject.GetComponent<Station_Controller>();
    }

    private void Start()
    {
        // subscriptions
        _controller.detection.EnterEvent += Toggle_AmountBar;
        _controller.detection.ExitEvent += Toggle_AmountBar;

        IInteractable_Controller interactable = _controller.iInteractable;
        FoodData_Controller foodIcon = _controller.Food_Icon();

        interactable.OnTriggerInteract += foodIcon.Show_Icon;
        interactable.OnTriggerInteract += foodIcon.Show_Condition;
        interactable.OnTriggerInteract += Toggle_AmountBar;
        interactable.OnTriggerInteract += Empty_Destroy;
        
        interactable.OnInteract += Transfer_Food;
        interactable.OnInteract += Empty_Destroy;
        
        interactable.OnAction1 += Transfer_Food;
        interactable.OnAction1 += Empty_Destroy;
        
        interactable.OnAction2 += Transfer_Food;
        interactable.OnAction2 += Empty_Destroy;

        interactable.OnHoldInteract += Transfer_Food;
        interactable.OnHoldInteract += Empty_Destroy;
    }

    private void OnDestroy()
    {
        // subscriptions
        _controller.detection.EnterEvent -= Toggle_AmountBar;
        _controller.detection.ExitEvent -= Toggle_AmountBar;
        
        IInteractable_Controller interactable = _controller.iInteractable;
        FoodData_Controller foodIcon = _controller.Food_Icon();

        interactable.OnTriggerInteract -= foodIcon.Show_Icon;
        interactable.OnTriggerInteract -= foodIcon.Show_Condition;
        interactable.OnTriggerInteract -= Toggle_AmountBar;
        interactable.OnTriggerInteract -= Empty_Destroy;
        
        interactable.OnInteract -= Transfer_Food;
        interactable.OnInteract -= Empty_Destroy;

        interactable.OnAction1 -= Transfer_Food;
        interactable.OnAction1 -= Empty_Destroy;
        
        interactable.OnAction2 -= Transfer_Food;
        interactable.OnAction2 -= Empty_Destroy;

        interactable.OnHoldInteract -= Transfer_Food;
        interactable.OnHoldInteract -= Empty_Destroy;
    }


    // Functions
    private void Toggle_AmountBar()
    {
        FoodData_Controller foodIcon = _controller.Food_Icon();
        foodIcon.Toggle_SubDataBar(_controller.detection.player != null);
    }


    private void Transfer_Food()
    {
        Player_Controller player = _controller.detection.player;
        
        if (player == null)
        {
            Toggle_AmountBar();
            Empty_Destroy();
            
            return;
        }
        
        FoodData_Controller playerIcon = player.foodIcon;
        if (playerIcon.DataCount_Maxed()) return;

        FoodData_Controller stationIcon = _controller.Food_Icon();

        // transfer to player
        playerIcon.Set_CurrentData(new(stationIcon.currentData.foodScrObj));
        playerIcon.currentData.Set_Condition(stationIcon.currentData.conditionDatas);

        playerIcon.Show_Icon();
        playerIcon.Show_Condition();
        playerIcon.Toggle_SubDataBar(true);

        // update current data & decrease one amount
        stationIcon.Set_CurrentData(null);
        Toggle_AmountBar();

        // sound
        Audio_Controller.instance.Play_OneShot(gameObject, 1);
    }

    private void Transfer_All()
    {
        FoodData_Controller playerIcon = _controller.detection.player.foodIcon;

        if (playerIcon.DataCount_Maxed()) return;

        FoodData_Controller stationIcon = _controller.Food_Icon();

        int transferCount = 0;

        for (int i = 0; i < stationIcon.currentData.currentAmount; i++)
        {
            playerIcon.Set_CurrentData(stationIcon.currentData);
            transferCount++;

            if (playerIcon.DataCount_Maxed()) break;
        }

        playerIcon.Show_Icon();
        playerIcon.Show_Condition();
        playerIcon.Toggle_SubDataBar(true);

        stationIcon.currentData.Update_Amount(-transferCount);
        stationIcon.Toggle_AmountBar(stationIcon.currentData.currentAmount > 1);

        // sound
        Audio_Controller.instance.Play_OneShot(gameObject, 2);
    }


    private void Empty_Destroy()
    {
        if (_controller.Food_Icon().AllDatas().Count > 0) return;

        OnDestroy();

        // clear current food data
        _controller.Food_Icon().Set_CurrentData(null);

        // station control
        Main_Controller main = Main_Controller.instance;

        main.currentStations.Remove(_controller);
        main.data.claimedPositions.Remove(transform.position);

        Audio_Controller.instance.Play_OneShot(gameObject, 0);

        Destroy(gameObject);
    }
}
