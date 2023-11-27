using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FoodStateIndicator_Data
{
    public FoodState_Type type;
    public SpriteRenderer sr;
    public List<Sprite> sprite = new();
}

public class FoodState_Indicator : MonoBehaviour
{
    private SpriteRenderer _sr;

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
            if (type != foodStateIndicatorDatas[i].type) continue;

            return foodStateIndicatorDatas[i];
        }
        return null;
    }

    // Custom
    private void Reset_State()
    {
        for (int i = 0; i < foodStateIndicatorDatas.Count; i++)
        {
            foodStateIndicatorDatas[i].sr.sprite = null;
        }
    }
    public void Update_StateSprite(List<FoodState_Data> foodStateData)
    {
        Reset_State();

        for (int i = 0; i < foodStateData.Count; i++)
        {
            FoodStateIndicator_Data data = Get_Data(foodStateData[i].stateType);
            data.sr.sprite = data.sprite[foodStateData[i].stateLevel - 1];
        }
    }
}