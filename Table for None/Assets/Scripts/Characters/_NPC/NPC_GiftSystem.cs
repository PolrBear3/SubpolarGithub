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
    public AmountBar coolTimeBar => _coolTimeBar;

    
    [Space(20)]
    [SerializeField][Range(0, 100)] private float _itemDropRate;
    [SerializeField][Range(0, 100)] private int _dropAmountRange;

    [Space(20)] 
    [SerializeField] private ActionSelector_Data[] _giftActionDatas;
    
    
    [Space(40)] 
    [SerializeField] private SpriteRenderer _recruitQuestIcon;
    [SerializeField] private AmountBar _recruitBar;
    
    [SerializeField] [Range(0, 100)] private float _recruitRate;

    
    [Space(40)]
    [SerializeField] private VideoGuide_Trigger _videoGuide;
    [SerializeField] private Guide_ScrObj _recruitGuide;
    [SerializeField] private Ability_ScrObj _buddyMergeCountAbility;


    public Action OnGift;
    
    public Action<bool> OnDurationToggle;
    private Coroutine _coroutine;
    
    
    private Sprite _emptyQuestSprite;
    
    private bool _isRecruiting;
    public bool isRecruiting => _isRecruiting;
    
    private Coroutine _recruitBarCoroutine;


    // UnityEngine
    private void Start()
    {
        _giftCoolTimeBar.SetActive(false);

        _coolTimeBar.Set_Amount(_coolTimeBar.maxAmount);
        _coolTimeBar.Toggle(true);

        // subscriptions
        _controller.interactable.OnHoldInteract += Gift;
        _controller.interactable.OnHoldInteract += ToggleBar_Duration;

        GlobalTime_Controller.instance.OnTimeTik += Update_CoolTime;

        _controller.interactable.OnInteract += _videoGuide.Trigger_CurrentGuide;
        
        if (gameObject.TryGetComponent(out Buddy_NPC buddyNPC)) return;
        if (_recruitQuestIcon == null) return;
        
        // reset data
        _emptyQuestSprite = _recruitQuestIcon.sprite;
        _recruitQuestIcon.gameObject.SetActive(false);
        _recruitBar.Toggle(false);
        
        // recruitment subscriptions
        _controller.interactable.OnHoldInteract += Transfer_RecruitQuestFood;

        NPC_GiftSystem giftSystem = _controller.giftSystem;
        if (giftSystem == null) return;

        _controller.giftSystem.coolTimeBar.OnMaxAmount += Cancel_Recruitment;
        _controller.giftSystem.OnGift += Toggle_Recruitment;
    }

    private void OnDestroy()
    {
        // subscriptions
        _controller.interactable.OnHoldInteract -= ToggleBar_Duration;
        _controller.interactable.OnHoldInteract -= Gift;

        GlobalTime_Controller.instance.OnTimeTik -= Update_CoolTime;
        
        _controller.interactable.OnInteract -= _videoGuide.Trigger_CurrentGuide;
        
        if (gameObject.TryGetComponent(out Buddy_NPC buddyNPC)) return;
        if (_recruitQuestIcon == null) return;
        
        // recruitment subscriptions
        _controller.interactable.OnHoldInteract -= Transfer_RecruitQuestFood;
        
        NPC_GiftSystem giftSystem = _controller.giftSystem;
        if (giftSystem == null) return;

        _controller.giftSystem.coolTimeBar.OnMaxAmount -= Cancel_Recruitment;
        _controller.giftSystem.OnGift -= Toggle_Recruitment;
    }


    // Indications
    private void ToggleBar_Duration()
    {
        if (_coroutine != null) return;
        if (_isRecruiting) return;
        if (_controller.foodInteraction.FoodInteraction_Active()) return;

        _coroutine = StartCoroutine(ToggleBar_Duration_Coroutine());
    }
    private IEnumerator ToggleBar_Duration_Coroutine()
    {
        Update_CoolTimBar();

        OnDurationToggle?.Invoke(false);
        _giftCoolTimeBar.SetActive(true);

        yield return new WaitForSeconds(_coolTimeBar.durationTime);

        OnDurationToggle?.Invoke(true);
        _giftCoolTimeBar.SetActive(false);

        _coroutine = null;
        yield break;
    }


    private void Update_CoolTime()
    {
        if (_coolTimeBar.Is_MaxAmount()) return;

        _coolTimeBar.Update_Amount(1);
        Update_CoolTimBar();
    }

    private void Update_CoolTimBar()
    {
        _coolTimeBar.Toggle_BarColor(_coolTimeBar.Is_MaxAmount());
        _coolTimeBar.Load(_coolTimeBar.currentAmount);
    }


    // Gift Action Datas
    public int Random_DropAmount()
    {
        return UnityEngine.Random.Range(1, _dropAmountRange + 1);
    }
    
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

        return new(dropFood, Random_DropAmount());
    }
    
    public void Drop_Food()
    {
        ItemDropper dropper = _controller.itemDropper;

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
        if (!_coolTimeBar.Is_MaxAmount()) return false;

        // food interaction active
        if (_controller.foodInteraction.FoodInteraction_Active()) return false;

        Main_Controller main = Main_Controller.instance;

        // check if current position is claimed
        if (main.data.Position_Claimed(Utility.SnapPosition(transform.position))) return false;

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
        
        OnGift?.Invoke();

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
    
    
    // Buddy Recruit
    private List<NPC_Controller> Current_BuddyNPCs()
    {
        List<GameObject> characters = Main_Controller.instance.currentCharacters;
        List<NPC_Controller> buddyNPCs = new();
        
        for (int i = 0; i < characters.Count; i++)
        {
            if (!characters[i].TryGetComponent(out Buddy_NPC buddyNPC)) continue;
            if (!characters[i].TryGetComponent(out NPC_Controller controller)) continue;
            
            buddyNPCs.Add(controller);
        }
        
        return buddyNPCs;
    }
    
    
    private void Toggle_Recruitment()
    {
        if (_isRecruiting) return;
        if (!Utility.Percentage_Activated(_recruitRate)) return;
        
        _isRecruiting = true;
        
        _recruitQuestIcon.gameObject.SetActive(true);
        _recruitQuestIcon.sprite = _emptyQuestSprite;
        
        _recruitBar.Toggle_BarColor(true);
        _recruitBar.Toggle(true);
        _recruitBar.Load();
        
        VideoGuide_Controller.instance.Trigger_Guide(_recruitGuide);
    }

    private void Cancel_Recruitment()
    {
        _isRecruiting = false;
        
        _recruitQuestIcon.gameObject.SetActive(false);
        _recruitBar.Toggle(false);

        _controller.foodIcon.Update_AllDatas(null);
    }
    
    
    private void Transfer_RecruitQuestFood()
    {
        if (!_isRecruiting) return;

        Player_Controller player = _controller.interactable.detection.player;
        FoodData_Controller playerIcon = player.foodIcon;
        
        if (!playerIcon.hasFood) return;

        FoodData playerFoodData = new(playerIcon.currentData);
        Food_ScrObj playerFood = playerIcon.currentData.foodScrObj;
        
        FoodData_Controller foodIcon = _controller.foodIcon;

        playerIcon.Set_CurrentData(null);
        playerIcon.Show_Icon();
        playerIcon.Toggle_SubDataBar(true);
        playerIcon.Show_Condition();

        _controller.giftSystem.coolTimeBar.Set_Amount(0);
        
        // different food
        if (!foodIcon.Is_SameFood(playerFood))
        {
            foodIcon.Set_CurrentData(null);
            foodIcon.Set_CurrentData(new(playerFood));
            
            _recruitQuestIcon.sprite = playerFood.sprite;
            
            _recruitBar.Set_Amount(1);
            _recruitBar.Load();

            return;
        }
        
        // same food
        _recruitBar.Update_Amount(1);

        if (!_recruitBar.Is_MaxAmount())
        {
            _recruitBar.Load();
            return;
        }
        Cancel_Recruitment();

        Main_Controller main = Main_Controller.instance;
        
        // raw food
        if (main.dataController.Is_RawFood(playerFood))
        {
            List<Food_ScrObj> ingredientFoods = main.dataController.AllFoods(playerFood);
            int randIndex = UnityEngine.Random.Range(0, ingredientFoods.Count);

            for (int i = 0; i < _controller.giftSystem.Random_DropAmount(); i++)
            {
                playerIcon.Set_CurrentData(new(ingredientFoods[randIndex]));
            }
            playerIcon.Show_Icon();
            playerIcon.Toggle_SubDataBar(true);
            playerIcon.Show_Condition();
            
            return;
        }
            
        Buddy_Controller buddyController = player.buddyController;
        GameObject spawnBuddy = Instantiate(buddyController.buddyNPC, transform.position, Quaternion.identity);
        
        Buddy_NPC buddy = spawnBuddy.GetComponent<Buddy_NPC>();
        buddyController.Track_CurrentBuddy(buddy);

        int abilityCount = player.abilityManager.data.Ability_ActivationCount(_buddyMergeCountAbility);
        int mergeCount = buddyController.defaultMergeCount + abilityCount;
         
        buddy.Set_Data(new(playerFoodData, mergeCount));
        buddy.Load_DataIndication();

        main.UnTrack_CurrentCharacter(gameObject);
        Destroy(gameObject);
    }
}