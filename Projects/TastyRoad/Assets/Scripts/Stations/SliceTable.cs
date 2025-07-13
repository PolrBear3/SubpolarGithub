using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class SliceTable : Table
{
    [Header("")]
    [SerializeField] private Rhythm_HitBox _rhythmHitBox;
    

    // UnityEngine
    private new void Start()
    {
        Audio_Controller.instance.Create_EventInstance(gameObject, 3);
        Toggle_SliceAction();

        // subscriptions
        _rhythmHitBox.OnHitSuccess += Slice;
        
        Detection_Controller detection = stationController.detection;

        detection.EnterEvent += Toggle_SliceAction;
        detection.ExitEvent += Toggle_SliceAction;

        IInteractable_Controller interactable = stationController.iInteractable;
        
        interactable.OnInteract += SwapFood;
        interactable.OnInteract += Toggle_SliceAction;
        
        interactable.OnHoldInteract += SwapFood;
        interactable.OnHoldInteract += Toggle_SliceAction;
    }

    private new void OnDestroy()
    {
        // subscriptions
        _rhythmHitBox.OnHitSuccess -= Slice;
        
        Detection_Controller detection = stationController.detection;

        detection.EnterEvent -= Toggle_SliceAction;
        detection.ExitEvent -= Toggle_SliceAction;
        
        IInteractable_Controller interactable = stationController.iInteractable;
        
        interactable.OnInteract -= SwapFood;
        interactable.OnInteract -= Toggle_SliceAction;
        
        interactable.OnHoldInteract -= SwapFood;
        interactable.OnHoldInteract -= Toggle_SliceAction;
    }


    // Actions
    private void Toggle_SliceAction()
    {
        bool playerExit = stationController.detection.player == null;
        
        if (playerExit || Slice_Available() == false)
        {
            _rhythmHitBox.Toggle(false);

            // sound stop
            Audio_Controller.instance.EventInstance(gameObject, 3).stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            return;
        }

        _rhythmHitBox.Toggle(true);

        // sound play
        Audio_Controller.instance.EventInstance(gameObject, 3).start();
    }


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
    
    public void Slice()
    {
        if (Slice_Available() == false) return;

        stationController.Food_Icon().currentData.Update_Condition(new FoodCondition_Data(FoodCondition_Type.sliced));
        stationController.Food_Icon().Show_Condition();

        Toggle_SliceAction();

        // durability
        Station_Maintenance maintenance = stationController.maintenance;
            
        maintenance.Update_Durability(-1);
        maintenance.Update_DurabilityBreak();
    }
}
