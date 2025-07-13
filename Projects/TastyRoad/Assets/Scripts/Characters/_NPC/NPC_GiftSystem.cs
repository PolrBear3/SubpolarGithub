using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_GiftSystem : MonoBehaviour
{
    [Space(20)]
    [SerializeField] private NPC_Controller _controller;

    [Space(20)]
    [SerializeField] private GameObject _giftCoolTimeBar;
    [SerializeField] private AmountBar _coolTimeBar;

    
    [Space(20)]
    [SerializeField][Range(0, 100)] private int _dropCoolTime;
    [SerializeField][Range(0, 100)] private int _dropAmountRange;

    [Space(20)] 
    [SerializeField][Range(0, 100)] private float _itemDropRate;
    [SerializeField] private ActionSelector_Data[] _giftActionDatas;

    [Space(40)] 
    [SerializeField] private VideoGuide_Trigger _videoGuide;


    public Action<bool> OnDurationToggle;
    private Coroutine _coroutine;


    // UnityEngine
    private void Start()
    {
        _giftCoolTimeBar.SetActive(false);

        _coolTimeBar.Set_Amount(_dropCoolTime);
        _coolTimeBar.Toggle(true);

        // subscriptions
        _controller.interactable.OnHoldInteract += ToggleBar_Duration;
        _controller.interactable.OnHoldInteract += Gift;

        globaltime.instance.OnTimeTik += Update_CoolTime;

        _controller.interactable.OnInteract += _videoGuide.Trigger_CurrentGuide;
    }

    private void OnDestroy()
    {
        // subscriptions
        _controller.interactable.OnHoldInteract -= ToggleBar_Duration;
        _controller.interactable.OnHoldInteract -= Gift;

        globaltime.instance.OnTimeTik -= Update_CoolTime;
        
        _controller.interactable.OnInteract -= _videoGuide.Trigger_CurrentGuide;
    }


    // Indications
    private void ToggleBar_Duration()
    {
        if (_coroutine != null) return;

        _coroutine = StartCoroutine(ToggleBar_Duration_Coroutine());
    }
    private IEnumerator ToggleBar_Duration_Coroutine()
    {
        Update_CoolTimBar();

        OnDurationToggle?.Invoke(false);
        _giftCoolTimeBar.SetActive(true);

        yield return new WaitForSeconds(2f);

        OnDurationToggle?.Invoke(true);
        _giftCoolTimeBar.SetActive(false);

        _coroutine = null;
        yield break;
    }


    private void Update_CoolTime()
    {
        if (_coolTimeBar.currentAmount >= _dropCoolTime) return;

        _coolTimeBar.Update_Amount(1);
        Update_CoolTimBar();
    }

    private void Update_CoolTimBar()
    {
        _coolTimeBar.Toggle_BarColor(_coolTimeBar.currentAmount >= _dropCoolTime);
        _coolTimeBar.Load_Custom(_dropCoolTime, _coolTimeBar.currentAmount);
    }


    // Gift Action Datas
    private ActionSelector_Data WeightRandom_GiftAction()
    {
        float totalWeight = 0f;

        // Calculate total weight
        foreach (var data in _giftActionDatas)
        {
            totalWeight += data.activateValue;
        }

        // If no weight, return null or random fallback
        if (totalWeight <= 0f) return null;

        float randomValue = UnityEngine.Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var data in _giftActionDatas)
        {
            cumulative += data.activateValue;
            if (randomValue <= cumulative)
            {
                return data;
            }
        }

        return null;
    }
    
    
    private FoodData FoodDrop()
    {
        FoodData_Controller playerFoodIcon = _controller.interactable.detection.player.foodIcon;
        if (playerFoodIcon.hasFood == false) return null;
        
        LocationData currentLocation = Main_Controller.instance.currentLocation.data;
        Food_ScrObj playerFood = playerFoodIcon.currentData.foodScrObj;
        
        Food_ScrObj randWeightedFood = currentLocation.WeightRandom_Food(playerFood);
        List<Food_ScrObj> foodIngredients = randWeightedFood.Ingredients();

        Food_ScrObj dropFood = foodIngredients[UnityEngine.Random.Range(0, foodIngredients.Count)];
        int dropAmount = UnityEngine.Random.Range(1, _dropAmountRange + 1);
        
        Debug.Log(dropFood + " " + dropAmount);
        return new(dropFood, dropAmount);
    }
    
    public void Drop_Food()
    {
        ItemDropper dropper = _controller.itemDropper;
        Debug.Log("Drop_Food");
        
        dropper.Drop_Food(FoodDrop());
    }
    
    
    // Actions
    private void Empty_PlayerFood()
    {
        FoodData_Controller playerFoodIcon = _controller.interactable.detection.player.foodIcon;
        
        playerFoodIcon.Set_CurrentData(null);
        playerFoodIcon.Show_Icon();
        playerFoodIcon.Toggle_SubDataBar(true);
        playerFoodIcon.Show_Condition();
    }
    
    
    private bool Gift_Available()
    {
        // check if cool time complete
        if (_coolTimeBar.currentAmount < _dropCoolTime) return false;

        // check if food serve waiting
        if (_controller.foodIcon.hasFood) return false;

        Main_Controller main = Main_Controller.instance;

        // check if current position is claimed
        if (main.Position_Claimed(Utility.SnapPosition(transform.position))) return false;

        FoodData_Controller playerFoodIcon = _controller.interactable.detection.player.foodIcon;

        // check if player has gift food
        if (playerFoodIcon.hasFood == false) return false;

        return true;
    }
    
    private void Gift()
    {
        if (Gift_Available() == false) return;

        Food_ScrObj playerFood = _controller.interactable.detection.player.foodIcon.currentData.foodScrObj;
        Empty_PlayerFood();
        
        // start cool time
        _coolTimeBar.Set_Amount(0);
        Update_CoolTimBar();

        // sound
        Audio_Controller audio = Audio_Controller.instance;
        audio.Play_OneShot(gameObject, 3);

        // check drop rate
        if (Utility.Percentage_Activated(_itemDropRate) == false) return;
        
        // sound
        audio.Play_OneShot(gameObject, 4);
        
        // drop gift
        ItemDropper dropper = _controller.itemDropper;
        
        if (Main_Controller.instance.dataController.Is_RawFood(playerFood))
        {
            dropper.Drop_Gold(UnityEngine.Random.Range(1, playerFood.price + 1));
            return;
        }
        dropper.Drop_CollectCard();
    }
}