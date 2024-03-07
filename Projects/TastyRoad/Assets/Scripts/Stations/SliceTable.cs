using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliceTable : MonoBehaviour, IInteractable, ISignal
{
    private Detection_Controller _detection;

    [SerializeField] private FoodData_Controller _foodIcon;
    [SerializeField] private Rhythm_HitBox _hitBox;



    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Detection_Controller detection)) { _detection = detection; }
    }



    // OnTrigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Player_Controller player)) return;

        if (_foodIcon.hasFood == false) return;
        _hitBox.Activate_HitBox();
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Player_Controller player)) return;

        _hitBox.Deactivate_HitBox();
    }



    // IInteractable
    public void Interact()
    {
        Swap_Food();

        if (_foodIcon.hasFood == false)
        {
            _hitBox.Deactivate_HitBox();
            return;
        }

        _hitBox.Activate_HitBox();
    }

    public void UnInteract()
    {

    }

    // ISignal
    public void Signal()
    {
        _foodIcon.Update_State(FoodState_Type.sliced, 1);
    }



    // Swap SliceTable and Player Food
    private void Swap_Food()
    {
        FoodData_Controller playerIcon = _detection.player.foodIcon;

        Food_ScrObj ovenFood = _foodIcon.currentFoodData.foodScrObj;
        List<FoodState_Data> ovenStateData = new(_foodIcon.currentFoodData.stateData);

        Food_ScrObj playerFood = playerIcon.currentFoodData.foodScrObj;
        List<FoodState_Data> playerStateData = new(playerIcon.currentFoodData.stateData);

        _foodIcon.Assign_Food(playerFood);
        _foodIcon.Assign_State(playerStateData);

        playerIcon.Assign_Food(ovenFood);
        playerIcon.Assign_State(ovenStateData);
    }
}
