using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollectCard : ItemDrop
{
    [Space(20)]
    [SerializeField] private CoinLauncher _launcher;

    
    private List<Action> OnPickups = new();
    private ItemSlot_Data _collectData;


    // UnityEngine
    private new void Start()
    {
        base.Start();
        
        interactable.OnInteract += Pickup;
    }

    private new void OnDestroy()
    {
        base.OnDestroy();
        
        interactable.OnInteract -= Pickup;
    }


    // Pickup Interaction
    public void Assign_Pickup(Action pickupAction)
    {
        if (pickupAction == null) return;
        
        OnPickups.Add(pickupAction);
    }

    public void Pickup()
    {
        if (OnPickups == null || OnPickups.Count <= 0)
        {
            Assign_Pickup(FoodIngredient_toArchive);
            Assign_Pickup(StationBluePrint_toArchive);
        }

        int randIndex = UnityEngine.Random.Range(0, OnPickups.Count);
        OnPickups[randIndex]?.Invoke();
    }


    // Food Ingredient
    public void Set_FoodIngredient(Food_ScrObj food)
    {
        _collectData = new(new FoodData(food));
    }

    public void SetLocation_FoodIngredient(bool set)
    {
        if (set == false) return;

        Location_Controller currentLocation = Main_Controller.instance.currentLocation;
        Set_FoodIngredient(currentLocation.data.WeightRandom_Food());
    }


    public void FoodIngredient_toArchive()
    {
        SetLocation_FoodIngredient(_collectData == null);
        
        ArchiveMenu_Controller menu = Main_Controller.instance.currentVehicle.menu.archiveMenu;
        Food_ScrObj archiveFood = _collectData.currentFood;

        ItemSlot_Data existingData = menu.Archived_FoodData(archiveFood);
        ItemSlot_Data newData = menu.Archive_Food(archiveFood);
        
        bool foodUnlocked = existingData != null && existingData.isLocked == false;
        
        menu.Unlock_BookmarkToggle(newData, foodUnlocked);
        menu.Unlock_FoodIngredient(archiveFood, 0);

        // dialog
        gameObject.GetComponent<DialogTrigger>().Update_Dialog(0);

        // pickup animation
        _launcher.Parabola_CoinLaunch(_launcher.setCoinSprites[0], detection.player.transform.position);
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

        Location_Controller currentLocation = Main_Controller.instance.currentLocation;
        Set_Blueprint(currentLocation.data.WeightRandom_Station());
    }


    public void StationBluePrint_toArchive()
    {
        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();
        StationMenu_Controller menu = Main_Controller.instance.currentVehicle.menu.stationMenu;

        // available slots check
        if (menu.controller.slotsController.Empty_SlotData(menu.ItemSlot_Datas()) == null)
        {
            dialog.Update_Dialog(2);
            return;
        }

        SetLocation_Blueprint(_collectData == null);

        // add station blueprint
        Station_ScrObj addStation = _collectData.currentStation;
        menu.Toggle_DataLock(menu.Add_StationItem(addStation, 1), true);

        // dialog
        dialog.Update_Dialog(1);

        // pickup
        _launcher.Parabola_CoinLaunch(_launcher.setCoinSprites[0], detection.player.transform.position);
        Destroy(gameObject, 0.1f);
    }
}
