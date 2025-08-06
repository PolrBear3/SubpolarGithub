using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Buddy_NPC : MonoBehaviour
{
    [Space(20)]
    [SerializeField] private NPC_Controller _controller;
    public NPC_Controller controller => _controller;

    [Space(20)] 
    [SerializeField] private AnimatorOverrideController[] _buddyAnimators;

    [Space(20)] 
    [SerializeField] [Range(0, 10)] private float _followSpacingDistance;
    [SerializeField] [Range(0, 100)] private float _followIncreaseSpeed;
    
    [Space(10)] 
    [SerializeField] private Station_ScrObj[] _pickupStations;
    [SerializeField] private Station_ScrObj[] _placeStations;


    private BuddyNPC_Data _data;
    public BuddyNPC_Data data => _data;
    
    private bool _isFollowing;
    public bool isFollowing => _isFollowing;
    
    private Coroutine _coroutine;
    
    
    // MonoBehaviour
    private void Start()
    {
        _controller.foodIcon.amountBar.Toggle(true);
        
        Load_SkinAnimator();
        Follow_Player();
    }


    // Data
    private void Load_SkinAnimator()
    {
        Buddy_Controller buddyController = Main_Controller.instance.Player().buddyController;

        List<AnimatorOverrideController> currentAnimators = new();
        foreach (Buddy_NPC buddy in buddyController.currentBuddies)
        {
            currentAnimators.Add(buddy.controller.basicAnim.animOverride);
        }
        
        List<AnimatorOverrideController> buddyAnimators = new();
        foreach (AnimatorOverrideController animator in _buddyAnimators)
        {
            buddyAnimators.Add(animator);
        }

        while (buddyAnimators.Count > 0)
        {
            int randIndex = Random.Range(0, buddyAnimators.Count);
            AnimatorOverrideController animator = buddyAnimators[randIndex];

            if (currentAnimators.Contains(animator))
            {
                buddyAnimators.Remove(animator);
                continue;
            }

            _controller.basicAnim.Set_OverrideController(animator);
            return;
        }
        
        _controller.basicAnim.Set_OverrideController(_buddyAnimators[Random.Range(0, _buddyAnimators.Length)]);
    }
    
    public void Set_Data(BuddyNPC_Data data)
    {
        _data = data;
    }
    
    public void Load_DataIndication()
    {
        FoodData_Controller foodIcon = _controller.foodIcon;
        
        foodIcon.Set_CurrentData(new(_data.automateFoodData));
        foodIcon.Hide_Condition();
        
        Update_AutomationBar();
    }
    
    
    // Indication
    private void Update_AutomationBar()
    {
        FoodData_Controller foodIcon = _controller.foodIcon;
        float transparency = _data.Ingredients_Collected() ? 100f : 50f;

        foodIcon.Show_Icon(transparency);
        foodIcon.amountBar.Load_Custom(_data.maxMergeCount, _data.currentMergeCount);
    }
    
    
    // Movements
    private void Follow_Player()
    {
        if (_coroutine != null) StopCoroutine(_coroutine);
        
        _isFollowing = true;

        NPC_Movement movement = _controller.movement;
        movement.Set_MoveSpeed(movement.defaultMoveSpeed + _followIncreaseSpeed);
        
        _coroutine = StartCoroutine(Follow_Coroutine());
    }
    private IEnumerator Follow_Coroutine()
    {
        Main_Controller main = Main_Controller.instance;
        Player_Controller player = main.Player();
        
        Buddy_Controller buddyController = player.buddyController;
        NPC_Movement movement = _controller.movement;
        
        while (_isFollowing)
        {
            if (IngredientPickup_Available())
            {
                Pickup_Ingredients();
                yield break;
            }

            if (Merge_Available())
            {
                Merge_CollectedIngredients();
                yield break;
            }
            
            yield return new WaitForSeconds(movement.intervalTime);

            Buddy_NPC followBuddy = buddyController.Follow_Buddy(this);
            Transform followTarget = followBuddy != null ? followBuddy.transform : player.transform;
            
            movement.Assign_TargetPosition(followTarget.position);
            
            if (Vector2.Distance(transform.position, followTarget.position) > _followSpacingDistance) continue;
            movement.Assign_TargetPosition(transform.position);
        }
    }
    
    
    // Ingredient Pickup
    private Station_Controller FoodPickup_Station(Food_ScrObj placedFood)
    {
        List<Station_Controller> stations = Main_Controller.instance.currentStations;

        stations.Sort((a, b) =>
        {
            float distA = Vector2.Distance(transform.position, a.transform.position);
            float distB = Vector2.Distance(transform.position, b.transform.position);
            return distA.CompareTo(distB);
        });
        
        for (int i = 0; i < stations.Count; i++)
        {
            if (_pickupStations.Contains(stations[i].stationScrObj) == false) continue;
            
            FoodData_Controller foodIcon = stations[i].Food_Icon();
            
            if (foodIcon == null) continue;
            if (foodIcon.Has_SameFood(placedFood) == false) continue;
            if (foodIcon.currentData.conditionDatas.Count != 0) continue;

            return stations[i];
        }
        return null;
    }
    
    private bool IngredientPickup_Available()
    {
        if (_data == null) return false;

        List<Food_ScrObj> foodIngredients = new(_data.automateFoodData.foodScrObj.Ingredients());

        for (int i = 0; i < foodIngredients.Count; i++)
        {
            if (_data.Ingredient_Collected(foodIngredients[i])) continue;
            if (FoodPickup_Station(foodIngredients[i]) == false) continue;

            return true;
        }
        return false;
    }

    private void Pickup_Ingredients()
    {
        if (_coroutine != null) StopCoroutine(_coroutine);

        _isFollowing = false;
        
        NPC_Movement movement = _controller.movement;
        movement.Set_MoveSpeed(0);
        
        _coroutine = StartCoroutine(Pickup_Coroutine());
    }
    private IEnumerator Pickup_Coroutine()
    {
        NPC_Movement movement = _controller.movement;
        List<Food_ScrObj> foodIngredients = new(_data.automateFoodData.foodScrObj.Ingredients());

        while (IngredientPickup_Available())
        {
            for (int i = 0; i < foodIngredients.Count; i++)
            {
                if (_data.Ingredient_Collected(foodIngredients[i])) continue;
                
                Station_Controller pickupStation = FoodPickup_Station(foodIngredients[i]);
                if (pickupStation == null) continue;

                movement.Assign_TargetPosition(pickupStation.transform.position);
                while (movement.At_TargetPosition() == false) yield return null;
                
                if (pickupStation == null) continue;

                FoodData_Controller stationIcon = pickupStation.Food_Icon();
                if (stationIcon == null || stationIcon.hasFood == false) continue;
                
                _data.collectedIngredients.Add(new(stationIcon.currentData));
                stationIcon.Set_CurrentData(null);

                Update_AutomationBar();
                
                if (pickupStation.TryGetComponent(out IInteractable interactable) == false) continue;
                interactable.Trigger_Interact();
            }
        }

        Follow_Player();
        yield break;
    }


    // Ingredients Merge
    private Station_Controller FoodPlace_Station()
    {
        List<Station_Controller> stations = Main_Controller.instance.CurrentStations();

        stations.Sort((a, b) =>
        {
            float distA = Vector2.Distance(transform.position, a.transform.position);
            float distB = Vector2.Distance(transform.position, b.transform.position);
            return distA.CompareTo(distB);
        });
        
        for (int i = 0; i < stations.Count; i++)
        {
            if (_placeStations.Contains(stations[i].stationScrObj) == false) continue;
            
            FoodData_Controller stationIcon = stations[i].Food_Icon();
            
            if (stationIcon == null) continue;
            if (stationIcon.DataCount_Maxed()) continue;
            
            return stations[i];
        }
        return null;
    }
    
    private bool Merge_Available()
    {
        if (_data == null || _data.Ingredients_Collected() == false) return false;
        if (FoodPlace_Station() == null) return false;
        
        return true;
    }
    
    private void Merge_CollectedIngredients()
    {
        if (_coroutine != null) StopCoroutine(_coroutine);

        _isFollowing = false;
        
        NPC_Movement movement = _controller.movement;
        movement.Set_MoveSpeed(0);
        
        _coroutine = StartCoroutine(Merge_Coroutine());
    }
    private IEnumerator Merge_Coroutine()
    {
        NPC_Movement movement = _controller.movement;
        
        while (Merge_Available())
        {
            Station_Controller placeStation = FoodPlace_Station();
            if (placeStation == null) continue;
            
            movement.Assign_TargetPosition(placeStation.transform.position);
            while (movement.At_TargetPosition() == false) yield return null;

            if (placeStation == null) continue;
            
            FoodData_Controller stationIcon = placeStation.Food_Icon();
            if (stationIcon.DataCount_Maxed()) continue;
            
            placeStation.Food_Icon().Set_CurrentData(new(_data.automateFoodData.foodScrObj));
            
            List<Food_ScrObj> ingredients = _data.automateFoodData.foodScrObj.Ingredients();
            foreach (Food_ScrObj ingredient in ingredients)
            {
                _data.Remove_CollectedIngredient(ingredient);
            }
            _data.Update_MergeCount(-1);
            
            Update_AutomationBar();
            
            if (placeStation.gameObject.TryGetComponent(out IInteractable interactable) == false) continue;
            interactable.Trigger_Interact();
        }

        if (_data.currentMergeCount <= 0)
        {
            Main_Controller.instance.Player().buddyController.Remove_Buddy(this);
            yield break;
        }
        
        Follow_Player();
        yield break;
    }
}