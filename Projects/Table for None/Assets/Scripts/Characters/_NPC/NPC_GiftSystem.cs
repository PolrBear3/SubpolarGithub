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


    public Action<bool> OnDurationToggle;
    
    private Coroutine _coroutine;
    private Coroutine _giftCoroutine;


    // UnityEngine
    private void Start()
    {
        _giftCoolTimeBar.SetActive(false);

        _coolTimeBar.Set_Amount(_coolTimeBar.maxAmount);
        _coolTimeBar.Toggle(true);
        
        // subscriptions
        _controller.interactable.OnHoldInteract += Gift_Gold;
        _controller.interactable.OnHoldInteract += ToggleBar_Duration;

        GlobalTime_Controller.instance.OnTimeTik += Update_CoolTime;
    }

    private void OnDestroy()
    {
        // subscriptions
        _controller.interactable.OnHoldInteract -= Gift_Gold;
        _controller.interactable.OnHoldInteract -= ToggleBar_Duration;

        GlobalTime_Controller.instance.OnTimeTik -= Update_CoolTime;
    }


    // Indications
    private void ToggleBar_Duration()
    {
        if (_coroutine != null) return;
        if (_controller.questSystem.QuestSystem_Active()) return;
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


    // Datas
    private int Random_DropAmount()
    {
        return UnityEngine.Random.Range(1, _dropAmountRange + 1);
    }

    private int Random_GoldAmount(Food_ScrObj giftFood)
    {
        return UnityEngine.Random.Range(giftFood.price / 2, giftFood.price + 1);
    }
    
    
    // Main
    private bool Gift_Available()
    {
        // check if cool time complete
        if (!_coolTimeBar.Is_MaxAmount()) return false;
        
        if (_controller.questSystem.QuestSystem_Active()) return false;
        if (_controller.foodInteraction.FoodInteraction_Active()) return false;

        // check if player has gift food
        FoodData_Controller playerFoodIcon = _controller.interactable.detection.player.foodIcon;
        if (playerFoodIcon.hasFood == false) return false;

        return true;
    }
    
    private void Empty_PlayerFood()
    {
        FoodData_Controller playerFoodIcon = _controller.interactable.detection.player.foodIcon;
        
        playerFoodIcon.Set_CurrentData(null);
        playerFoodIcon.Show_Icon();
        playerFoodIcon.Toggle_SubDataBar(true);
        playerFoodIcon.Show_Condition();
    }
    
    
    // Food Drop
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
    
    public void Gift_Food()
    {
        ItemDropper dropper = _controller.itemDropper;

        dropper.Drop_Food(FoodDrop());
    }
    
    
    // Gold Drop
    private void Gift_Gold()
    {
        if (Gift_Available() == false) return;

        // gift food
        Food_ScrObj playerFood = _controller.interactable.detection.player.foodIcon.currentData.foodScrObj;
        Empty_PlayerFood();
        
        // check drop rate
        if (Utility.Percentage_Activated(_itemDropRate) == false) return;
        
        // set drop amount
        int goldAmount = Random_GoldAmount(playerFood);
        if (goldAmount <= 0) return;
        
        _controller.interactable.LockInteract(true);
        _giftCoroutine = StartCoroutine(GolfGift_Coroutine(goldAmount));
    }
    private IEnumerator GolfGift_Coroutine(int goldAmount)
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

        // drop gold
        ItemDropper dropper = _controller.itemDropper;
        
        dropper.Set_DropPosition(dropPos);
        dropper.Drop_Gold(goldAmount);
        
        // start cool time
        _coolTimeBar.Set_Amount(0);
        Update_CoolTimBar();
        
        // sound
        Audio_Controller audio = Audio_Controller.instance;
        audio.Play_OneShot(gameObject, 3);
        
        _controller.interactable.LockInteract(false);
        _giftCoroutine = null;
    }
    
    
    // Buddy Recruit
    /*
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
    */
}