using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliceTable : Table, IInteractable, ISignal
{
    [SerializeField] private Rhythm_HitBox _hitBox;



    // OnTrigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (stationController.movement.enabled) return;
        if (!collision.TryGetComponent(out Player_Controller player)) return;

        if (stationController.Food_Icon().hasFood == false) return;

        stationController.PlayerInput_Activation(true);
        _hitBox.Activate_HitBox();
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (stationController.movement.enabled) return;
        if (!collision.TryGetComponent(out Player_Controller player)) return;

        stationController.PlayerInput_Activation(false);
        _hitBox.Deactivate_HitBox();
    }



    // IInteractable
    public new void Interact()
    {
        Basic_SwapFood();

        if (stationController.Food_Icon().hasFood == false)
        {
            _hitBox.Deactivate_HitBox();
            return;
        }

        _hitBox.Activate_HitBox();
    }



    // ISignal
    public void Signal()
    {
        FoodData_Controller icon = stationController.Food_Icon();

        if (icon.hasFood == false) return;

        icon.currentData.Update_Condition(new FoodCondition_Data(FoodCondition_Type.sliced));
        icon.Show_Condition();
    }
}
