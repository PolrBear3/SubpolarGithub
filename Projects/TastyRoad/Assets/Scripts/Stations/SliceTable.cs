using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class SliceTable : Table, IInteractable
{
    [Header("")]
    [SerializeField] private ActionKey _actionKey;

    [SerializeField] private InputActionReference[] _actionRefs;

    private bool _sliceActive;


    // UnityEngine
    private new void Start()
    {
        base.Start();

        Audio_Controller.instance.Create_EventInstance(gameObject, 3);
        Toggle_SliceAction();

        // subscriptions
        Detection_Controller detection = stationController.detection;

        detection.EnterEvent += Toggle_SliceAction;
        detection.ExitEvent += Toggle_SliceAction;
    }

    private new void OnDestroy()
    {
        base.OnDestroy();

        // subscriptions
        Detection_Controller detection = stationController.detection;

        detection.EnterEvent -= Toggle_SliceAction;
        detection.ExitEvent -= Toggle_SliceAction;

        Toggle_SliceSubscription(false);
    }


    // IInteractable
    public new void Interact()
    {
        Basic_SwapFood();

        Toggle_SliceAction();
    }

    public new void Hold_Interact()
    {
        base.Hold_Interact();

        Toggle_SliceAction();
    }


    // Actions
    private bool Slice_Available()
    {
        FoodData_Controller foodIcon = stationController.Food_Icon();
        if (!foodIcon.hasFood) return false;

        FoodCondition_Type[] restrictions = foodIcon.currentData.foodScrObj.restrictedCondtions;
        if (restrictions.Contains(FoodCondition_Type.sliced)) return false;

        int currentLevel = foodIcon.currentData.Current_ConditionLevel(FoodCondition_Type.sliced);
        if (currentLevel >= 3) return false;

        return true;
    }

    private void Set_SliceAction()
    {
        _actionKey.Reset_CurrentKey();

        // get random action
        int randIndex = Random.Range(0, _actionRefs.Length);
        InputActionReference randRef = _actionRefs[randIndex];

        // set selected random action key
        _actionKey.Set_CurrentKey(randRef);
    }


    private void Toggle_SliceSubscription(bool toggle)
    {
        Input_Controller input = Input_Controller.instance;

        if (toggle == false)
        {
            input.OnAnyInput -= Slice;
            _sliceActive = false;

            return;
        }

        if (_sliceActive) return;

        input.OnAnyInput += Slice;
        _sliceActive = true;
    }

    private void Toggle_SliceAction()
    {
        bool playerExit = stationController.detection.player == null;

        if (playerExit || Slice_Available() == false)
        {
            _actionKey.Reset_CurrentKey();
            Toggle_SliceSubscription(false);

            // sound stop
            Audio_Controller.instance.EventInstance(gameObject, 3).stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

            return;
        }

        Set_SliceAction();
        Toggle_SliceSubscription(true);

        // sound play
        Audio_Controller.instance.EventInstance(gameObject, 3).start();
    }


    public void Slice(InputActionReference sliceActionRef)
    {
        if (Slice_Available() == false) return;
        if (sliceActionRef != _actionKey.currentReference) return;

        stationController.Food_Icon().currentData.Update_Condition(new FoodCondition_Data(FoodCondition_Type.sliced));
        stationController.Food_Icon().Show_Condition();

        Toggle_SliceAction();

        // durability
        stationController.data.Update_Durability(-1);
        stationController.maintenance.Update_DurabilityBreak();
    }
}
