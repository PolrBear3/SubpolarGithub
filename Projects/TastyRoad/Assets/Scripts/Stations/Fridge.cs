using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Fridge : Stack_Table
{
    [Space(20)] 
    [SerializeField] private AmountBar _delayTimeBar;
    [SerializeField][Range(0, 100)] private float _freezeDelayTime;

    private Coroutine _coroutine;

    
    // UnityEngine
    public new void Start()
    {
        base.Start();
        Freeze_Food();
        
        // subscriptions
        Detection_Controller detection = stationController.detection;
        
        detection.EnterEvent += () => _delayTimeBar.Toggle(DelayBar_ToggleAvailable());
        detection.ExitEvent += () => _delayTimeBar.Toggle(DelayBar_ToggleAvailable());

        IInteractable_Controller interactable = stationController.iInteractable;
        
        interactable.OnInteract += Freeze_Food;
        interactable.OnHoldInteract += Freeze_Food;
        
        interactable.OnAction1 += Freeze_Food;
        interactable.OnAction2 += Cancel_FreezeCoroutine;
    }

    public new void OnDestroy()
    {
        base.OnDestroy();
        
        // subscriptions
        Detection_Controller detection = stationController.detection;
        
        detection.EnterEvent -= () => _delayTimeBar.Toggle(DelayBar_ToggleAvailable());
        detection.ExitEvent -= () => _delayTimeBar.Toggle(DelayBar_ToggleAvailable());
        
        IInteractable_Controller interactable = stationController.iInteractable;
        
        interactable.OnInteract -= Freeze_Food;
        interactable.OnHoldInteract -= Freeze_Food;
        
        interactable.OnAction1 -= Freeze_Food;
        interactable.OnAction2 -= Cancel_FreezeCoroutine;
    }


    // Main
    private bool DelayBar_ToggleAvailable()
    {
        return _coroutine != null && stationController.detection.player == null;
    }
    
    private bool AllFood_Frozen()
    {
        FoodData_Controller foodIcon = stationController.Food_Icon();
        List<FoodData> datas = foodIcon.AllDatas();

        for (int i = 0; i < datas.Count; i++)
        {
            if (datas[i].Current_ConditionLevel(FoodCondition_Type.frozen) < 3) return false;
        }
        return true;
    }


    private void Cancel_FreezeCoroutine()
    {
        if (stationController.Food_Icon().hasFood && !AllFood_Frozen()) return;
        
        _delayTimeBar.Toggle(false);
        
        if (_coroutine == null) return;
        
        StopCoroutine(_coroutine);
        _coroutine = null;
    }
    
    private void Freeze_Food()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }

        FoodData_Controller foodIcon = stationController.Food_Icon();

        if (!foodIcon.hasFood || AllFood_Frozen())
        {
            _delayTimeBar.Toggle(false);
            return;
        }
        _delayTimeBar.Toggle_BarColor(true);

        // restrict rotten system
        foodIcon.Toggle_TikCount(false);

        _coroutine = StartCoroutine(Freeze_Coroutine());
    }
    private IEnumerator Freeze_Coroutine()
    {
        FoodData_Controller foodIcon = stationController.Food_Icon();

        while (foodIcon.hasFood && AllFood_Frozen() == false)
        {
            _delayTimeBar.Set_Amount(0);
            _delayTimeBar.Toggle(DelayBar_ToggleAvailable());
            
            float delayTikTime = _freezeDelayTime / _delayTimeBar.maxAmount;
                
            while (_delayTimeBar.Is_MaxAmount() == false)
            {
                _delayTimeBar.Update_Amount(1);
                _delayTimeBar.Load();
                
                yield return new WaitForSeconds(delayTikTime);
            }

            List<FoodData> datas = foodIcon.AllDatas();

            foreach (FoodData data in datas)
            {
                data.Update_Condition(new FoodCondition_Data(FoodCondition_Type.frozen));
            }

            foodIcon.Update_AllDatas(datas);
            foodIcon.Show_Condition();

            // durability
            Station_Maintenance maintenance = stationController.maintenance;
            
            maintenance.Update_Durability(-1);
            maintenance.Update_DurabilityBreak();
        }
        
        _delayTimeBar.Toggle(false);

        _coroutine = null;
        yield break;
    }
}