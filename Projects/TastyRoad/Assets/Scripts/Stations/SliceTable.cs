using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SliceTable : Table, IInteractable
{
    [Header("")]
    [SerializeField] private GameObject _allActionKeys;
    [SerializeField] private GameObject[] _actionKeys;


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
    private void Update_ActionKey()
    {
        foreach (GameObject actionKey in _actionKeys)
        {
            actionKey.SetActive(false);
        }

        Input_Controller input = Input_Controller.instance;

        if (_allActionKeys.activeSelf == false)
        {
            input.OnAction1 -= Slice;
            input.OnAction2 -= Slice;
        }

        int randKeyIndex = Random.Range(0, _actionKeys.Length);

        // indicate random action key //
        // update slice action subscriptions //
    }


    private bool Slice_Available()
    {
        FoodData_Controller foodIcon = stationController.Food_Icon();
        if (!foodIcon.hasFood) return false;

        FoodCondition_Type[] restrictions = foodIcon.currentData.foodScrObj.restrictedCondtions;
        if (restrictions.Contains(FoodCondition_Type.sliced)) return false;

        return true;
    }


    private void Toggle_SliceAction()
    {
        bool playerExit = stationController.detection.player == null;

        if (playerExit || Slice_Available() == false)
        {
            _allActionKeys.SetActive(false);
            Update_ActionKey();

            // sound stop
            Audio_Controller.instance.EventInstance(gameObject, 3).stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

            return;
        }

        _allActionKeys.SetActive(true);
        Update_ActionKey();

        // sound play
        Audio_Controller.instance.EventInstance(gameObject, 3).start();
    }

    public void Slice()
    {
        if (!Slice_Available()) return;

        stationController.Food_Icon().currentData.Update_Condition(new FoodCondition_Data(FoodCondition_Type.sliced));
        stationController.Food_Icon().Show_Condition();

        // durability
        stationController.data.Update_Durability(-1);
        stationController.maintenance.Update_DurabilityBreak();
    }
}
