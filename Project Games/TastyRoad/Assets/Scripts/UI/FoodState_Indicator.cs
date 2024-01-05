using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FoodStateIndicator_Data
{
    public FoodState_Type stateType;
    public List<Sprite> sprite = new();
}

public class FoodState_Indicator : MonoBehaviour
{
    private SpriteRenderer _sr;

    public List<SpriteRenderer> _stateBoxSR = new();
    public List<FoodStateIndicator_Data> foodStateIndicatorDatas = new();

    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out SpriteRenderer sr)) { _sr = sr; }
    }

    // Get
    public FoodStateIndicator_Data Get_Data(FoodState_Type type)
    {
        for (int i = 0; i < foodStateIndicatorDatas.Count; i++)
        {
            if (type != foodStateIndicatorDatas[i].stateType) continue;

            return foodStateIndicatorDatas[i];
        }
        return null;
    }

    // Custom
    public void Reset_State()
    {
        for (int i = 0; i < _stateBoxSR.Count; i++)
        {
            _stateBoxSR[i].sprite = null;
        }
    }

    public void Update_StateSprite(List<FoodState_Data> foodStateData)
    {
        Reset_State();

        for (int i = 0; i < foodStateData.Count; i++)
        {
            FoodStateIndicator_Data data = Get_Data(foodStateData[i].stateType);
            _stateBoxSR[i].sprite = data.sprite[foodStateData[i].stateLevel - 1];
        }
    }
    public void Update_StateSprite(List<FoodState_Data> foodStateData, FoodState_Type stateType)
    {
        Reset_State();

        for (int i = 0; i < foodStateData.Count; i++)
        {
            if (foodStateData[i].stateType != stateType) continue;

            FoodStateIndicator_Data data = Get_Data(foodStateData[i].stateType);
            _stateBoxSR[0].sprite = data.sprite[foodStateData[i].stateLevel - 1];
        }
    }
}