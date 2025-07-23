using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buddy_NPC : MonoBehaviour
{
    [Space(20)]
    [SerializeField] private NPC_Controller _controller;
    public NPC_Controller controller => _controller;

    [Space(20)] 
    [SerializeField] [Range(0, 10)] private float _playerDistanceRange;


    private FoodData _automateFoodData;
    public FoodData automateFoodData => _automateFoodData;
    
    private Coroutine _coroutine;
    
    
    // MonoBehaviour
    private void Start()
    {
        Player_Follow();
    }


    // Movements
    private void Player_Follow()
    {
        _coroutine = StartCoroutine(Follow_Coroutine());
    }
    private IEnumerator Follow_Coroutine()
    {
        Main_Controller main = Main_Controller.instance;
        Player_Controller player = main.Player();
        
        Buddy_Controller buddyController = player.buddyController;
        NPC_Movement movement = _controller.movement;
        
        while (true)
        {
            yield return new WaitForSeconds(movement.intervalTime);

            int buddyNum = buddyController.currentBuddies.IndexOf(this);
            Transform followTarget = buddyNum == 0 ? player.transform : buddyController.currentBuddies[buddyNum - 1].transform;
            
            movement.Assign_TargetPosition(followTarget.position);
            
            if (Vector2.Distance(transform.position, followTarget.position) > _playerDistanceRange) continue;
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
    }
    
    // share components
    // - food data controller
    // - IInteractable_Controller

    private bool AutomateCook_Available()
    {
        // get ingredients
        // check all stations with ingredients
        
        return true;
    }
}
