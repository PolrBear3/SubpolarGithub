using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliceTable : Table, IInteractable, ISignal
{
    [SerializeField] private Rhythm_HitBox _hitBox;


    // UnityEngine
    private new void Start()
    {
        base.Start();

        Audio_Controller.instance.Create_EventInstance(gameObject, 2);
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
        Audio_Controller.instance.EventInstance(gameObject, 2).start();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (stationController.movement.enabled) return;
        if (!collision.TryGetComponent(out Player_Controller player)) return;

        stationController.PlayerInput_Activation(false);
        _hitBox.Deactivate_HitBox();

        // sound stop
        Audio_Controller.instance.EventInstance(gameObject, 2).stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }


    // IInteractable
    public new void Interact()
    {
        Basic_SwapFood();

        Update_Effects();
    }

    public new void Hold_Interact()
    {
        base.Hold_Interact();

        Update_Effects();
    }


    // ISignal
    public void Signal()
    {
        if (stationController.Food_Icon().hasFood == false) return;

        stationController.Food_Icon().currentData.Update_Condition(new FoodCondition_Data(FoodCondition_Type.sliced));
        stationController.Food_Icon().Show_Condition();

        // durability
        stationController.data.Update_Durability(-1);
        stationController.maintenance.Update_DurabilityBreak();
    }


    //
    private void Update_Effects()
    {
        if (stationController.Food_Icon().hasFood == false)
        {
            _hitBox.Deactivate_HitBox();

            // sound stop
            Audio_Controller.instance.EventInstance(gameObject, 2).stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

            return;
        }

        _hitBox.Activate_HitBox();

        // sound play
        Audio_Controller.instance.EventInstance(gameObject, 2).start();
    }
}
