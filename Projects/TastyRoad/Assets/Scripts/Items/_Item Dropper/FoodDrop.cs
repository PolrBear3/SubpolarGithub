using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodDrop : ItemDrop
{
    [Space(20)]
    [SerializeField] private FoodData_Controller _foodIcon;
    public FoodData_Controller foodIcon => _foodIcon;


    // UnityEngine
    private new void Start()
    {
        base.Start();
        
        _foodIcon.SetMax_SubDataCount(_foodIcon.AllDatas().Count);
        AmountBar_Toggle();

        // subscriptions
        interactable.OnInteract += Pickup;
        interactable.OnHoldInteract += Pickup_All;

        detection.EnterEvent += AmountBar_Toggle;
        detection.ExitEvent += AmountBar_Toggle;
    }

    private new void OnDestroy()
    {
        base.OnDestroy();
        
        // subscriptions
        interactable.OnInteract -= Pickup;
        interactable.OnHoldInteract -= Pickup_All;
        
        detection.EnterEvent -= AmountBar_Toggle;
        detection.ExitEvent -= AmountBar_Toggle;
    }


    // Indication
    private void AmountBar_Toggle()
    {
        _foodIcon.Toggle_SubDataBar(detection.player != null);
    }
    
    
    // Food Pickup
    private bool Transfer_Available()
    {
        if (_foodIcon.hasFood == false) return false;
        
        FoodData_Controller playerIcon = detection.player.foodIcon;
        if (playerIcon.DataCount_Maxed()) return false;

        return true;
    }
    
    private bool Transfer()
    {
        if (_foodIcon.hasFood == false)
        {
            Destroy(gameObject);
            return false;
        }

        FoodData_Controller playerIcon = detection.player.foodIcon;

        if (playerIcon.DataCount_Maxed()) return false;

        playerIcon.Set_CurrentData(_foodIcon.currentData);
        playerIcon.Show_Icon();
        playerIcon.Show_Condition();
        playerIcon.Toggle_SubDataBar(true);

        _foodIcon.Set_CurrentData(null);
        _foodIcon.Toggle_SubDataBar(true);

        if (_foodIcon.hasFood) return true;
        
        Destroy(gameObject);
        return true;
    }
    
    
    private void Pickup()
    {
        if (Transfer_Available())
        {
            Audio_Controller.instance.Play_OneShot(gameObject, 0);
            TutorialQuest_Controller.instance.Complete_Quest("FoodPickup", 1);
        }
        
        if (Transfer() == false) return;

        Audio_Controller.instance.Play_OneShot(gameObject, 0);
    }

    private void Pickup_All()
    {
        if (_foodIcon.hasFood == false)
        {
            Destroy(gameObject);
            return;
        }

        FoodData_Controller playerIcon = detection.player.foodIcon;
        int pickupAmount = _foodIcon.AllDatas().Count;

        if (pickupAmount <= 0) return;

        if (Transfer_Available())
        {
            Audio_Controller.instance.Play_OneShot(gameObject, 0);
            TutorialQuest_Controller.instance.Complete_Quest("FoodPickup", 1);
        }
        
        for (int i = 0; i < pickupAmount; i++)
        {
            if (playerIcon.DataCount_Maxed()) return;
            Transfer();
        }
    }
}