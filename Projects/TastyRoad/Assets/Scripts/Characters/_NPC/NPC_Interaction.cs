using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Interaction : MonoBehaviour
{
    [Space(20)]
    [SerializeField] private NPC_Controller _controller;

    [Space(20)] 
    [SerializeField] private SpriteRenderer _recruitQuestIcon;
    [SerializeField] private AmountBar _recruitBar;
    
    [SerializeField] [Range(0, 100)] private float _recruitRate;
    

    private Sprite _emptyQuestSprite;
    
    private bool _isRecruiting;
    public bool isRecruiting => _isRecruiting;
    
    private Coroutine _recruitBarCoroutine;


    // UnityEngine
    private void Start()
    {
        // subscriptions
        ActionBubble_Interactable interactable = _controller.interactable;

        interactable.OnInteract += Interact_FacePlayer;
        interactable.OnHoldInteract += Interact_FacePlayer;

        if (gameObject.TryGetComponent(out Buddy_NPC buddyNPC)) return;
        
        // reset data
        _emptyQuestSprite = _recruitQuestIcon.sprite;
        _recruitQuestIcon.gameObject.SetActive(false);
        _recruitBar.Toggle(false);
        
        // recruitment subscriptions
        interactable.OnHoldInteract += Transfer_RecruitQuestFood;

        NPC_GiftSystem giftSystem = _controller.giftSystem;

        _controller.giftSystem.coolTimeBar.OnMaxAmount += Cancel_Recruitment;
        _controller.giftSystem.OnGift += Toggle_Recruitment;
    }

    private void OnDestroy()
    {
        // subscriptions
        ActionBubble_Interactable interactable = _controller.interactable;

        interactable.OnInteract -= Interact_FacePlayer;
        interactable.OnHoldInteract -= Interact_FacePlayer;

        // recruitment subscriptions
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
        Buddy_Controller buddyController = player.buddyController;

        // spawn buddy
        GameObject spawnBuddy = Instantiate(player.buddyController.buddyNPC, transform.position, Quaternion.identity);
        
        Buddy_NPC buddy = spawnBuddy.GetComponent<Buddy_NPC>();
        buddyController.Track_CurrentBuddy(buddy);

        int mergeCount = buddyController.defaultMergeCount; // + ability //
         
        buddy.Set_Data(new(playerFoodData, mergeCount));
        buddy.Load_DataIndication();

        main.UnTrack_CurrentCharacter(gameObject);
        Destroy(gameObject);
    }
}