using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliceTable : Table, IInteractable, ISignal
{
    [SerializeField] private Rhythm_HitBox _hitBox;


    // UnityEngine
    private void Start()
    {
        Audio_Controller.instance.Create_EventInstance("SliceTable_slice", gameObject);
    }

    private new void OnDestroy()
    {
        base.OnDestroy();

        // sound
        Audio_Controller.instance.Remove_EventInstance(gameObject);
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
        Audio_Controller.instance.EventInstance(gameObject).start();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (stationController.movement.enabled) return;
        if (!collision.TryGetComponent(out Player_Controller player)) return;

        stationController.PlayerInput_Activation(false);
        _hitBox.Deactivate_HitBox();

        // sound stop
        Audio_Controller.instance.EventInstance(gameObject).stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }


    // IInteractable
    public new void Interact()
    {
        Basic_SwapFood();

        if (stationController.Food_Icon().hasFood == false)
        {
            _hitBox.Deactivate_HitBox();

            // sound stop
            Audio_Controller.instance.EventInstance(gameObject).stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

            return;
        }

        _hitBox.Activate_HitBox();

        // sound play
        Audio_Controller.instance.EventInstance(gameObject).start();
    }


    // ISignal
    public void Signal()
    {
        if (stationController.Food_Icon().hasFood == false) return;

        stationController.Food_Icon().currentData.Update_Condition(new FoodCondition_Data(FoodCondition_Type.sliced));
        stationController.Food_Icon().Show_Condition();
    }
}
