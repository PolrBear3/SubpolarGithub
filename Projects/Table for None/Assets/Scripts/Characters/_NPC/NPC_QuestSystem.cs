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
    public bool questActive => _questActive;

    private int _prizeIndex;

    private int _goldPaymentQuestPrice;

    private Food_ScrObj _foodIngredientPrize;
    private Station_ScrObj _stationBlueprintPrize;
    
    
    // MonoBehaviour
    private void Start()
    {
        Activate_Quest();
        _prizeIcon.gameObject.SetActive(_questActive);
        
        // subscriptions
        _controller.interactable.OnAction1 += Complete_Quest;
    }

    private void OnDestroy()
    {
        // subscriptions
        _controller.interactable.OnAction1 -= Complete_Quest;
    }


    // Indication
    private void Toggle_PrizeIcon(Sprite prizeSprite)
    {
        _prizeIcon.gameObject.SetActive(prizeSprite != null);
        
        if (prizeSprite == null) return;
        _prizeIcon.sprite = prizeSprite;
    }
    
    
    // Main
    private int QuestActive_Count()
    {
        List<NPC_Controller> currentNPCs = Main_Controller.instance.currentLocation.Current_npcControllers();
        int count = 0;

        for (int i = 0; i < currentNPCs.Count; i++)
        {
            if (currentNPCs[i].questSystem.questActive == false) continue;
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
    }

    private void Complete_Quest()
    {
        if (_questActive == false) return;

        FoodData_Controller foodIcon = _controller.foodIcon;

        if (foodIcon.hasFood == false)
        {
            // gold payment quest

            _questActive = false;
            return;
        }
        
        // food transfer quest
        
        _questActive = false;
    }
    
    
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
        if (_foodIngredientPrize != null)
        {
            // drop food ingredient
            
            return;
        }
        
        // drop blueprint
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