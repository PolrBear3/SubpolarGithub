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
    [SerializeField] [Range(0, 10)] private float _followSpacingDistance;
    [SerializeField] private Station_ScrObj[] _pickupStations;


    private bool _isFollowing;
    public bool isFollowing => _isFollowing;
    
    private FoodData _automateFoodData;
    public FoodData automateFoodData => _automateFoodData;

    private Coroutine _coroutine;
    
    
    // MonoBehaviour
    private void Start()
    {
        Follow_Player();
    }


    // Movements
    private void Follow_Player()
    {
        if (_coroutine != null) StopCoroutine(_coroutine);
        
        _isFollowing = true;
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
            if (AutomateIngredients_Placed())
            {
                Collect_AutomateIngredients();
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
    
    
    // Cook Automation
    public void Set_AutomateFoodData(FoodData foodData)
    {
        _automateFoodData = foodData;

        FoodData_Controller foodIcon = _controller.foodIcon;
        
        foodIcon.Set_CurrentData(new(foodData));
        foodIcon.Show_Icon(50);
        
        foodIcon.Hide_Condition();
        // foodIcon.Show_Condition();
    }

    
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

            return stations[i];
        }
        return null;
    }
    
    private bool AutomateIngredients_Placed()
    {
        List<Food_ScrObj> foodIngredients = new(_automateFoodData.foodScrObj.Ingredients());

        for (int i = foodIngredients.Count - 1; i >= 0; i--)
        {
            if (FoodPickup_Station(foodIngredients[i]) == null) continue;
            foodIngredients.RemoveAt(i);
        }
        
        return foodIngredients.Count == 0;
    }

    private void Collect_AutomateIngredients()
    {
        if (_coroutine != null) StopCoroutine(_coroutine);

        _isFollowing = false;
        _coroutine = StartCoroutine(Collect_Coroutine());
    }
    private IEnumerator Collect_Coroutine()
    {
        NPC_Movement movement = _controller.movement;
        
        List<Food_ScrObj> foodIngredients = new(_automateFoodData.foodScrObj.Ingredients());
        List<FoodData> collectedFoodDatas = new();

        while (foodIngredients.Count != collectedFoodDatas.Count)
        {
            for (int i = 0; i < foodIngredients.Count; i++)
            {
                Station_Controller pickupStation = FoodPickup_Station(foodIngredients[i]);
                if (pickupStation == null) continue;
            
                movement.Assign_TargetPosition(pickupStation.transform.position);
                
                while (movement.At_TargetPosition() == false) yield return null;
                yield return new WaitForSeconds(movement.intervalTime);
            
                if (pickupStation == null) continue;
                
                FoodData_Controller stationIcon = pickupStation.Food_Icon();
                if (stationIcon.Is_SameFood(foodIngredients[i]) == false) continue;
                
                stationIcon.Set_CurrentData(null);
                stationIcon.Show_Icon();
                
                collectedFoodDatas.Add(new(foodIngredients[i]));
            }

            bool ingredientFound = false;
            
            for (int i = 0; i < foodIngredients.Count; i++)
            {
                for (int j = 0; j < collectedFoodDatas.Count; j++)
                {
                    if (foodIngredients[i] == collectedFoodDatas[j].foodScrObj) continue;
                    if (FoodPickup_Station(foodIngredients[i]) == null) continue;

                    ingredientFound = true;
                    break;
                }
                if (ingredientFound) break;
            }
            
            if (ingredientFound) continue;
            break;
        }

        // change this to cook food //
        Follow_Player();
        yield break;
    }
}
