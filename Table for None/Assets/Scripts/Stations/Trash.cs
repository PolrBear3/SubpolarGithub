using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Trash : Stack_Table
{
    [Header("")]
    [SerializeField] private UnityEvent_Data[] _rewardDropActions;
    [SerializeField] private StationWeight_Data[] _dropStations;


    // MonoBehaviour
    private new void Start()
    {
        stationController.Food_Icon().ShowIcon_LockToggle(true);
        AmountBar_Toggle();
        
        // subscriptions
        IInteractable_Controller interactable = stationController.iInteractable;
        
        interactable.OnInteract += Interact;
        
        interactable.OnHoldInteract += PlayAnimation_TrashFood;
        interactable.OnHoldInteract += Trash_AllFood;
        interactable.OnHoldInteract += AmountBar_Toggle;
        
        stationController.maintenance.OnDurabilityBreak += Drop_CurrentFood;
    }

    private new void OnDestroy()
    {
        // subscriptions
        IInteractable_Controller interactable = stationController.iInteractable;
        
        interactable.OnInteract -= Interact;
        
        interactable.OnHoldInteract -= PlayAnimation_TrashFood;
        interactable.OnHoldInteract -= Trash_AllFood;
        interactable.OnHoldInteract -= AmountBar_Toggle;
        
        stationController.maintenance.OnDurabilityBreak -= Drop_CurrentFood;
    }


    // IInteractable_Controller
    private void Interact()
    {
        PlayAnimation_TrashFood();

        if (!Drop_Reward())
        {
            Trash_Food();
        }

        AmountBar_Toggle();
    }


    // Indications
    public new void AmountBar_Toggle()
    {
        FoodData_Controller trashFoodIcon = stationController.Food_Icon();
        AmountBar bar = trashFoodIcon.amountBar;

        bar.Toggle_BarColor(trashFoodIcon.DataCount_Maxed());
        base.AmountBar_Toggle();
    }


    // Trash Food
    private bool TrashFood_Available()
    {
        FoodData_Controller playerFoodIcon = stationController.detection.player.foodIcon;
        if (!playerFoodIcon.hasFood) return false;

        FoodData_Controller trashFoodIcon = stationController.Food_Icon();
        if (trashFoodIcon.DataCount_Maxed()) return false;

        return true;
    }


    private bool Trash_Food()
    {
        if (!TrashFood_Available()) return false;

        FoodData_Controller playerFoodIcon = stationController.detection.player.foodIcon;
        FoodData_Controller trashIcon = stationController.Food_Icon();

        FoodData trashData = new(playerFoodIcon.currentData);

        playerFoodIcon.Set_CurrentData(null);
        playerFoodIcon.Show_Icon();
        playerFoodIcon.Show_Condition();
        playerFoodIcon.Toggle_SubDataBar(true);

        trashIcon.Set_CurrentData(trashData);

        return true;
    }

    private void Trash_AllFood()
    {
        int repeatAmount = stationController.detection.player.foodIcon.AllDatas().Count;

        for (int i = 0; i < repeatAmount; i++)
        {
            if (!Trash_Food()) return;
        }
    }


    private void PlayAnimation_TrashFood()
    {
        if (!TrashFood_Available()) return;

        stationController.animController.Play_Animation("TransparencyBlinker_play");
    }

    private void Empty_MaxAmount()
    {
        FoodData_Controller trashFoodIcon = stationController.Food_Icon();
        if (!trashFoodIcon.DataCount_Maxed()) return;

        trashFoodIcon.Update_AllDatas(null);
    }


    // Reward
    private bool RewardDrop_Available()
    {
        FoodData_Controller trashFoodIcon = stationController.Food_Icon();
        if (!trashFoodIcon.DataCount_Maxed()) return false;

        return true;
    }

    private List<Vector2> RewardDrop_Positions()
    {
        Main_Controller main = Main_Controller.instance;
        Location_Controller currentLocation = main.currentLocation;

        List<Vector2> centeredPositions = Utility.Surrounding_SnapPositions(transform.position);
        List<Vector2> dropPositions = new();

        for (int i = 0; i < centeredPositions.Count; i++)
        {
            if (main.data.Position_Claimed(centeredPositions[i])) continue;
            if (currentLocation.Is_OuterSpawnPoint(centeredPositions[i])) continue;

            dropPositions.Add(centeredPositions[i]);
        }

        return dropPositions;
    }


    private UnityEvent RandomWeight_DropAction()
    {
        if (_rewardDropActions.Length <= 0) return null;

        float totalWeight = 0f;

        // Calculate total weight
        foreach (UnityEvent_Data data in _rewardDropActions)
        {
            totalWeight += data.probability;
        }

        if (totalWeight <= 0) return null;

        // Select a weighted random action
        float randValue = Random.Range(0, totalWeight);
        float cumulativeWeight = 0f;

        foreach (UnityEvent_Data data in _rewardDropActions)
        {
            cumulativeWeight += data.probability;

            if (randValue > cumulativeWeight) continue;
            return data.action;
        }

        return null;
    }

    private bool Drop_Reward()
    {
        if (!RewardDrop_Available()) return false;

        List<Vector2> dropPositions = RewardDrop_Positions();
        if (dropPositions.Count <= 0) return false;

        Vector2 dropPos = dropPositions[0];

        ItemDropper item = stationController.itemDropper;
        item.Set_DropPosition(dropPos);

        RandomWeight_DropAction()?.Invoke();
        Empty_MaxAmount();

        // durability
        Station_Maintenance maintenance = stationController.maintenance;
            
        maintenance.Update_Durability(-1);
        maintenance.Update_DurabilityBreak();

        return true;
    }


    // Reward Drops
    private List<FoodData> FoodDrops()
    {
        List<FoodData> foodDrops = new();

        Data_Controller data = Main_Controller.instance.dataController;

        FoodData_Controller food = stationController.Food_Icon();
        List<FoodData> currentDatas = food.AllDatas();

        int dropCount = 0;

        for (int i = 0; i < currentDatas.Count; i++)
        {
            if (data.Is_RawFood(currentDatas[i].foodScrObj)) continue;

            int randIndex = Random.Range(0, currentDatas[i].foodScrObj.Ingredients().Count);
            Food_ScrObj randIngredient = currentDatas[i].foodScrObj.Ingredients()[randIndex];

            foodDrops.Add(new(randIngredient));
            dropCount++;
        }

        if (dropCount <= 0)
        {
            Food_ScrObj defaultFood = stationController.itemDropper.foodWeights[0].foodScrObj;
            foodDrops.Add(new(defaultFood));
        }

        return foodDrops;
    }

    public void Drop_Food()
    {
        FoodData_Controller food = stationController.Food_Icon();
        List<FoodData> currentDatas = food.AllDatas();

        FoodData recentData = currentDatas[food.AllDatas().Count - 1];
        int rottenLevel = recentData.Current_ConditionLevel(FoodCondition_Type.rotten);

        // if (rottenLevel <= 0) return;

        List<FoodData> foodDrops = FoodDrops();
        FoodCondition_Data condition = new(FoodCondition_Type.rotten, rottenLevel);

        foreach (FoodData data in foodDrops)
        {
            data.Update_Condition(condition);
        }

        stationController.itemDropper.Drop_Food(foodDrops);
    }


    public void Drop_Station()
    {
        if (_dropStations.Length <= 0) return;

        float totalWeight = 0f;

        // Calculate total weight
        foreach (StationWeight_Data data in _dropStations)
        {
            totalWeight += data.weight;
        }

        if (totalWeight <= 0) return;

        // Select a weighted random action
        float randValue = Random.Range(0, totalWeight);
        float cumulativeWeight = 0f;

        for (int i = 0; i < _dropStations.Length; i++)
        {
            cumulativeWeight += _dropStations[i].weight;

            if (randValue > cumulativeWeight) continue;

            GameObject spawnStation = Instantiate(_dropStations[i].stationScrObj.prefab, RewardDrop_Positions()[0], Quaternion.identity);
            Station_Movement stationMovement = spawnStation.GetComponent<Station_Movement>();

            stationMovement.Load_Position();
            return;
        }
    }
}