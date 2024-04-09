using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliceTable : MonoBehaviour, IInteractable, ISignal
{
    private Station_Controller _controller;

    [SerializeField] private Rhythm_HitBox _hitBox;



    // UnityEngine
    private void Awake()
    {
        _controller = gameObject.GetComponent<Station_Controller>();
    }



    // OnTrigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_controller.movement.enabled) return;
        if (!collision.TryGetComponent(out Player_Controller player)) return;

        if (_controller.Food_Icon().hasFood == false) return;

        _controller.PlayerInput_Activation(true);
        _hitBox.Activate_HitBox();
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_controller.movement.enabled) return;
        if (!collision.TryGetComponent(out Player_Controller player)) return;

        _controller.PlayerInput_Activation(false);
        _hitBox.Deactivate_HitBox();
    }



    // IInteractable
    public void Interact()
    {
        Swap_Food();

        if (_controller.Food_Icon().hasFood == false)
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
        _controller.Food_Icon().Update_State(FoodState_Type.sliced, 1);
    }



    // Swap SliceTable and Player Food
    private void Swap_Food()
    {
        FoodData_Controller playerIcon = _controller.detection.player.foodIcon;
        FoodData_Controller tableIcon = _controller.Food_Icon();

        FoodData playerData = playerIcon.currentFoodData;
        FoodData tableData = tableIcon.currentFoodData;

        FoodData tempData = new(tableData);

        tableIcon.Assign_Food(playerData);
        playerIcon.Assign_Food(tempData);
    }
}
