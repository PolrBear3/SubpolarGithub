using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Interaction : MonoBehaviour
{
    [Space(20)]
    [SerializeField] private NPC_Controller _controller;

    [Space(20)] 
    [SerializeField] private GameObject _buddyNPC;
    [SerializeField] [Range(0, 100)] private float _recruitRate;
    
    [Space(10)] 
    [SerializeField] private SpriteRenderer _recruitQuestIcon;
    [SerializeField] private AmountBar _recruitBar;


    private bool _isRecruiting;
    public bool isRecruiting => _isRecruiting;
    
    private Coroutine _recruitBarCoroutine;


    // UnityEngine
    private void Start()
    {
        ActionBubble_Interactable interactable = _controller.interactable;

        interactable.OnInteract += Interact_FacePlayer;
        interactable.OnHoldInteract += Interact_FacePlayer;

        if (gameObject.TryGetComponent(out Buddy_NPC buddyNPC)) return;
        
        interactable.OnHoldInteract += Transfer_RecruitQuestFood;

        NPC_GiftSystem giftSystem = _controller.giftSystem;

        _controller.giftSystem.coolTimeBar.OnMaxAmount += Cancel_Recruitment;
        _controller.giftSystem.OnGift += Toggle_Recruitment;
    }

    private void OnDestroy()
    {
        ActionBubble_Interactable interactable = _controller.interactable;

        interactable.OnInteract -= Interact_FacePlayer;
        interactable.OnHoldInteract -= Interact_FacePlayer;

        if (gameObject.TryGetComponent(out Buddy_NPC buddyNPC)) return;
        
        interactable.OnHoldInteract -= Transfer_RecruitQuestFood;
        
        NPC_GiftSystem giftSystem = _controller.giftSystem;

        _controller.giftSystem.coolTimeBar.OnMaxAmount -= Cancel_Recruitment;
        _controller.giftSystem.OnGift -= Toggle_Recruitment;
    }


    // Main
    private void Interact_FacePlayer()
    {
        NPC_Movement movement = _controller.movement;

        movement.Stop_FreeRoam();
        _controller.basicAnim.Flip_Sprite(_controller.interactable.detection.player.gameObject);

        if (movement.isLeaving == true)
        {
            movement.Leave(movement.intervalTime);
            return;
        }

        movement.CurrentLocation_FreeRoam(movement.currentRoamArea, movement.intervalTime);
    }
    
    
    // Buddy Recruit
    private Food_ScrObj Random_RecruitQuestFood()
    {
        ArchiveMenu_Controller archiveMenu = Main_Controller.instance.currentVehicle.menu.archiveMenu;
        List<FoodData> unlockedFoodDatas = archiveMenu.ingredientUnlocks;

        if (unlockedFoodDatas.Count == 0) return null;
        
        int randIndex = Random.Range(0, unlockedFoodDatas.Count);
        return unlockedFoodDatas[randIndex].foodScrObj;
    }
    
    
    private void Toggle_Recruitment()
    {
        if (_isRecruiting) return;
        if (!Utility.Percentage_Activated(_recruitRate)) return;
       
        FoodData_Controller foodIcon = _controller.foodIcon;
        Food_ScrObj questFood = Random_RecruitQuestFood();

        if (questFood == null) return;
        
        _isRecruiting = true;
        
        foodIcon.Set_CurrentData(new(questFood));
        _recruitQuestIcon.sprite = foodIcon.currentData.foodScrObj.sprite;
        
        _recruitBar.transform.parent.gameObject.SetActive(true);
        _recruitBar.Toggle_BarColor(true);
        _recruitBar.Toggle(true);
        _recruitBar.Load();
    }

    private void Cancel_Recruitment()
    {
        _isRecruiting = false;
        
        _controller.foodIcon.Update_AllDatas(null);
        _recruitBar.transform.parent.gameObject.SetActive(false);
    }
    
    
    private void Transfer_RecruitQuestFood()
    {
        if (!_isRecruiting) return;

        FoodData_Controller playerIcon = _controller.interactable.detection.player.foodIcon;
        if (!playerIcon.hasFood) return;

        FoodData_Controller foodIcon = _controller.foodIcon;
        if (!foodIcon.Is_SameFood(playerIcon.currentData.foodScrObj)) return;
        
        playerIcon.Set_CurrentData(null);
        playerIcon.Show_Icon();
        playerIcon.Toggle_SubDataBar(true);
        playerIcon.Show_Condition();
        
        _recruitBar.Update_Amount(1);

        if (_recruitBar.Is_MaxAmount())
        {
            Cancel_Recruitment();
            
            Main_Controller main = Main_Controller.instance;
            
            // spawn buddy
            GameObject spawnBuddy = Instantiate(_buddyNPC, transform.position, Quaternion.identity);
            spawnBuddy.transform.SetParent(main.characterFile);
            
            main.UnTrack_CurrentCharacter(gameObject);
            Destroy(gameObject);

            return;
        }

        foodIcon.Set_CurrentData(new(Random_RecruitQuestFood()));
        _recruitQuestIcon.sprite = foodIcon.currentData.foodScrObj.sprite;

        _recruitBar.Load();

        // reset gift drop cooltime
        _controller.giftSystem.coolTimeBar.Set_Amount(0);
    }
}