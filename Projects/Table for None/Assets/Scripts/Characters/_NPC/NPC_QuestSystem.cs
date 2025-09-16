using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_QuestSystem : MonoBehaviour
{
    [Space(20)]
    [SerializeField] private NPC_Controller _controller;

    [Space(20)] 
    [SerializeField] private SpriteRenderer _prizeIcon;

    [Space(20)] 
    [SerializeField][Range(0, 100)] private int _npcMaxQuestCount;
    [SerializeField][Range(0, 100)] private float _questActiveRate;

    [Space(10)] 
    [SerializeField] [Range(0, 100)] private int _maxFoodTransferCount;
    
    [Space(20)] 
    [SerializeField] private ActionSelector_Data[] _prizes;


    private bool _questActive;
    private bool _questComplete;
    
    private int _prizeIndex;
    
    private int _goldPaymentQuestPrice;
    private int _foodTrasferQuestAmount;

    private Food_ScrObj _foodIngredientPrize;
    private Station_ScrObj _stationBlueprintPrize;

    private Coroutine _coroutine;
    
    
    // MonoBehaviour
    private void Start()
    {
        Activate_Quest();
        _prizeIcon.gameObject.SetActive(_questActive);
        
        // subscriptions
        ActionBubble_Interactable interactable = _controller.interactable;

        _controller.interactable.OnInteract += Update_BubbleIndication;
        
        _controller.interactable.OnHoldInteract += Complete_Quest;
        _controller.interactable.OnAction1 += Complete_Quest;
        
        _controller.interactable.OnHoldInteract += Drop_CurrentPrize;
        _controller.interactable.OnAction1 += Drop_CurrentPrize;
    }

    private void OnDestroy()
    {
        // subscriptions
        ActionBubble_Interactable interactable = _controller.interactable;

        _controller.interactable.OnInteract -= Update_BubbleIndication;
        
        _controller.interactable.OnHoldInteract -= Complete_Quest;
        _controller.interactable.OnAction1 -= Complete_Quest;
        
        _controller.interactable.OnHoldInteract -= Drop_CurrentPrize;
        _controller.interactable.OnAction1 -= Drop_CurrentPrize;
    }


    // Indication
    private void Toggle_PrizeIcon(Sprite prizeSprite)
    {
        _prizeIcon.gameObject.SetActive(prizeSprite != null);
        
        if (prizeSprite == null) return;
        _prizeIcon.sprite = prizeSprite;
    }
    public void Toggle_PrizeIcon(bool toggle)
    {
        _prizeIcon.gameObject.SetActive(toggle && _questActive);
    }

    private void Update_BubbleIndication()
    {
        Action_Bubble bubble = _controller.interactable.bubble;

        if (_questActive == false || _controller.foodInteraction.FoodInteraction_Active())
        {
            bubble.Empty_Bubble();
            bubble.Set_IndicatorToggleDatas(null);
            return;
        }
        
        if (_controller.foodIcon.hasFood == false)
        {
            Sprite goldSprite = GoldSystem.instance.defaultIcon;
            string goldPayString = _goldPaymentQuestPrice + " <sprite=56> " + bubble.bubbleDatas[0].LocalizedInfo();
            ActionBubble_Data goldBubbleData = new(goldSprite, goldPayString);

            bubble.Set_Bubble(goldSprite, null);
            bubble.Set_IndicatorToggleDatas(new(){ goldBubbleData });
            
            return;
        }
        
        FoodData_Controller foodIcon = _controller.foodIcon;
        
        Food_ScrObj currentFood = foodIcon.currentData.foodScrObj;
        string foodTransferString = currentFood.LocalizedName() + " " + bubble.bubbleDatas[0].LocalizedInfo();

        int completeCount = _foodTrasferQuestAmount - foodIcon.AllDatas().Count;
        string completeCountString = "[ " + completeCount + "/" + _foodTrasferQuestAmount + " ]  ";
        
        ActionBubble_Data foodBubbleData = new(currentFood.sprite, completeCountString + foodTransferString);
        
        bubble.Set_Bubble(currentFood.sprite, null);
        bubble.Set_IndicatorToggleDatas(new(){ foodBubbleData });
    }
    
    
    // Get
    private int QuestActive_Count()
    {
        List<NPC_Controller> currentNPCs = Main_Controller.instance.currentLocation.Current_npcControllers();
        int count = 0;

        for (int i = 0; i < currentNPCs.Count; i++)
        {
            NPC_QuestSystem questSystem = currentNPCs[i].questSystem;
            if (questSystem._questActive == false && questSystem._questComplete == false) continue;
            
            count++;
        }
        return count;
    }
    
    private bool Active_Available()
    {
        if (_questActive) return false;
        if (_controller.foodInteraction.FoodInteraction_Active()) return false;
        if (QuestActive_Count() >= _npcMaxQuestCount) return false;
        if (Utility.Percentage_Activated(_questActiveRate) == false) return false;
        
        return true;
    }

    public bool QuestSystem_Active()
    {
        return _questActive || _coroutine != null;
    }


    private int GoldPayment_QuestPrice()
    {
        if (_foodIngredientPrize != null)
        {
            return UnityEngine.Random.Range(_foodIngredientPrize.price, _foodIngredientPrize.price * 2);
        }
        return UnityEngine.Random.Range(_stationBlueprintPrize.price, _stationBlueprintPrize.price * 2);
    }

    private List<FoodData> FoodTransfer_QuestDatas()
    {
        LocationData currentLocationData = Main_Controller.instance.currentLocation.data;
        
        List<Food_ScrObj> ingredients = currentLocationData.WeightRandom_Food().Ingredients();
        List<FoodData> transferDatas = new();

        int dataCount = UnityEngine.Random.Range(1, _maxFoodTransferCount + 1);

        for (int i = 0; i < dataCount; i++)
        {
            int randIndex = UnityEngine.Random.Range(0, ingredients.Count);
            transferDatas.Add(new(ingredients[randIndex]));
        }
        
        return transferDatas;
    }

    
    // Quest Control
    private void Activate_Quest()
    {
        if (Active_Available() == false) return;
        
        _questActive = true;
        Set_RandomPrize();

        Action_Bubble bubble = _controller.interactable.bubble;

        if (UnityEngine.Random.Range(0, 2) == 0)
        {
            // gold payment quest
            _goldPaymentQuestPrice = GoldPayment_QuestPrice();
            return;
        }
        
        // food transfer quest
        FoodData_Controller foodIcon = _controller.foodIcon;
        
        foodIcon.Update_AllDatas(FoodTransfer_QuestDatas());
        foodIcon.Hide_Icon();

        _foodTrasferQuestAmount = foodIcon.AllDatas().Count;
    }
    
    private void Toggle_CompleteQuest()
    {
        _questComplete = true;
        _prizeIcon.gameObject.SetActive(false);
        
        Action_Bubble bubble = _controller.interactable.bubble;
        
        bubble.Empty_Bubble();
        bubble.Set_IndicatorToggleDatas(null);
    }
    
    private void Complete_Quest()
    {
        if (_questActive == false) return;
        if (_controller.foodInteraction.FoodInteraction_Active()) return;

        // gold payment quest
        FoodData_Controller foodIcon = _controller.foodIcon;
        if (foodIcon.hasFood == false)
        {
            if (GoldSystem.instance.Update_CurrentAmount(-_goldPaymentQuestPrice) == false) return;

            Toggle_CompleteQuest();
            return;
        }
        
        // food transfer quest
        FoodData_Controller playerIcon = Main_Controller.instance.Player().foodIcon;
        if (playerIcon.hasFood == false) return;
        
        Food_ScrObj questFood = foodIcon.currentData.foodScrObj;
        if (playerIcon.Has_SameFood(questFood) == false) return;

        playerIcon.Empty_TargetData(questFood);
        playerIcon.Show_Icon();
        playerIcon.Show_Condition();
        playerIcon.Toggle_SubDataBar(true);
        
        foodIcon.Set_CurrentData(null);
        
        if (foodIcon.hasFood) return;
        Toggle_CompleteQuest();
    }
    
    
    // Prize Control
    private void Set_RandomPrize()
    {
        if (_questActive == false) return;
        if (_prizes.Length == 0) return;
        
        float totalWeight = 0f;
        foreach (var prize in _prizes)
        {
            totalWeight += prize.activateValue;
        }

        float randomValue = UnityEngine.Random.Range(0f, totalWeight);

        float cumulative = 0f;
        for (int i = 0; i < _prizes.Length; i++)
        {
            cumulative += _prizes[i].activateValue;
            if (randomValue > cumulative) continue;

            _prizeIndex = i;
            break;
        }

        _prizes[_prizeIndex].actionEvent?.Invoke();
    }

    private void Drop_CurrentPrize()
    {
        if (_questActive == false || _questComplete == false) return;
        
        _controller.interactable.LockInteract(true);
        _coroutine = StartCoroutine(DropPrize_Coroutine());
    }
    private IEnumerator DropPrize_Coroutine()
    {
        Main_Controller main = Main_Controller.instance;
        Location_Controller location = Main_Controller.instance.currentLocation;
        NPC_Movement movement = _controller.movement;

        Vector2 dropPos = location.All_SpawnPositions(transform.position)[0];

        movement.Stop_FreeRoam();
        movement.Assign_TargetPosition(dropPos);

        while (movement.At_TargetPosition(dropPos) == false)
        {
            if (main.data.Position_Claimed(dropPos))
            {
                dropPos = location.All_SpawnPositions(transform.position)[0];
                movement.Assign_TargetPosition(dropPos);
            }
            yield return null;
        }
        movement.CurrentLocation_FreeRoam(0f);
        
        _controller.interactable.LockInteract(false);
        _questActive = false;
        
        // set collect card
        ItemDropper prizeDropper = _controller.itemDropper;
        
        prizeDropper.Set_DropPosition(dropPos);
        CollectCard prizeDrop = prizeDropper.DropReturn_CollectCard();
        
        // drop food ingredient
        if (_foodIngredientPrize != null)
        {
            prizeDrop.Set_FoodIngredient(_foodIngredientPrize);
            prizeDrop.Assign_Pickup(prizeDrop.FoodIngredient_toArchive);

            _coroutine = null;
            yield break;
        }
        
        // drop blueprint
        prizeDrop.Set_Blueprint(_stationBlueprintPrize);
        prizeDrop.Assign_Pickup(prizeDrop.StationBluePrint_toArchive);

        _coroutine = null;
    }
    
    
    // Prizes
    public void SetPrize_FoodIngredient()
    {
        LocationData currentData = Main_Controller.instance.currentLocation.data;
        Food_ScrObj randFood = currentData.WeightRandom_Food();
        
        _foodIngredientPrize = randFood;
        Toggle_PrizeIcon(_foodIngredientPrize.sprite);
    }
    
    public void SetPrize_StationBlueprint()
    {
        LocationData currentData = Main_Controller.instance.currentLocation.data;
        Station_ScrObj randBlueprint = currentData.WeightRandom_Station();
        
        _stationBlueprintPrize = randBlueprint;
        Toggle_PrizeIcon(_stationBlueprintPrize.dialogIcon);
    }
}