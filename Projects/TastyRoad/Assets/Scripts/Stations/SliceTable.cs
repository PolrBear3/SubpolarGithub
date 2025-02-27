using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SliceTable : Table, IInteractable, ISignal
{
    [SerializeField] private Rhythm_HitBox _hitBox;


    // UnityEngine
    private new void Start()
    {
        base.Start();

        Audio_Controller.instance.Create_EventInstance(gameObject, 3);
    }

    private new void OnDestroy()
    {
        base.OnDestroy();
    }


    // OnTrigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (stationController.movement.enabled) return;
        if (!collision.TryGetComponent(out Player_Controller player)) return;

        if (stationController.Food_Icon().hasFood == false) return;

        stationController.PlayerInput_Activation(true);
        _hitBox.Activate_HitBox();

        // sound play
        Audio_Controller.instance.EventInstance(gameObject, 3).start();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (stationController.movement.enabled) return;
        if (!collision.TryGetComponent(out Player_Controller player)) return;

        stationController.PlayerInput_Activation(false);
        _hitBox.Deactivate_HitBox();

        // sound stop
        Audio_Controller.instance.EventInstance(gameObject, 3).stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }


    // IInteractable
    public new void Interact()
    {
        Basic_SwapFood();

        ToggleSlice_HitBox();
    }

    public new void Hold_Interact()
    {
        base.Hold_Interact();

        ToggleSlice_HitBox();
    }


    // ISignal
    public void Signal()
    {
        if (!Slice_Available()) return;

        stationController.Food_Icon().currentData.Update_Condition(new FoodCondition_Data(FoodCondition_Type.sliced));
        stationController.Food_Icon().Show_Condition();

        // durability
        stationController.data.Update_Durability(-1);
        stationController.maintenance.Update_DurabilityBreak();
    }


    // SliceTable
    private bool Slice_Available()
    {
        FoodData_Controller foodIcon = stationController.Food_Icon();
        if (!foodIcon.hasFood) return false;

        FoodCondition_Type[] restrictions = foodIcon.currentData.foodScrObj.restrictedCondtions;
        if (restrictions.Contains(FoodCondition_Type.sliced)) return false;

        return true;
    }

    private void ToggleSlice_HitBox()
    {
        if (Slice_Available())
        {
            _hitBox.Activate_HitBox();

            // sound play
            Audio_Controller.instance.EventInstance(gameObject, 3).start();

            return;
        }

        _hitBox.Deactivate_HitBox();

        // sound stop
        Audio_Controller.instance.EventInstance(gameObject, 3).stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
