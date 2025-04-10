using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fridge : Stack_Table, IInteractable
{
    [Header("")]
    [SerializeField][Range(0, 100)] private float _freezeDelayTime;

    private Coroutine _coroutine;

    
    // UnityEngine
    public new void Start()
    {
        base.Start();
        Freeze_Food();
    }
    

    // IInteractable
    public new void Interact()
    {
        base.Interact();

        Freeze_Food();
    }

    public new void Hold_Interact()
    {
        base.Hold_Interact();

        Freeze_Food();
    }


    //
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
    
    private void Freeze_Food()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }

        FoodData_Controller foodIcon = stationController.Food_Icon();

        if (!foodIcon.hasFood) return;
        if (AllFood_Frozen()) return;

        // restrict rotten system
        foodIcon.Toggle_TikCount(false);

        _coroutine = StartCoroutine(Freeze_Food_Coroutine());
    }
    private IEnumerator Freeze_Food_Coroutine()
    {
        FoodData_Controller foodIcon = stationController.Food_Icon();

        while (foodIcon.hasFood && AllFood_Frozen() == false)
        {
            yield return new WaitForSeconds(_freezeDelayTime);

            List<FoodData> datas = foodIcon.AllDatas();

            foreach (FoodData data in datas)
            {
                data.Update_Condition(new FoodCondition_Data(FoodCondition_Type.frozen));
            }

            foodIcon.Update_AllDatas(datas);
            foodIcon.Show_Condition();

            // durability
            stationController.data.Update_Durability(-1);
            stationController.maintenance.Update_DurabilityBreak();
        }

        _coroutine = null;
        yield break;
    }
}