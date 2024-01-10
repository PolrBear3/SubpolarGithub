using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodState_Controller : MonoBehaviour
{
    [SerializeField] private Transform _stateIndicator;
    [SerializeField] private GameObject _stateBox;

    [HideInInspector] public List<FoodState_Box> currentStateBoxes = new();

    private float _boxPositionCount;

    // Get
    private FoodState_Box FoodState_Box(FoodState_Type stateType)
    {
        for (int i = 0; i < currentStateBoxes.Count; i++)
        {
            if (stateType == currentStateBoxes[i].currentData.stateType)
            {
                return currentStateBoxes[i];
            }
        }

        return null;
    }

    // Check
    public bool Has_State(FoodState_Type stateType)
    {
        for (int i = 0; i < currentStateBoxes.Count; i++)
        {
            if (stateType == currentStateBoxes[i].currentData.stateType) return true;
        }

        return false;
    }

    // State Boxes Position Order Update
    private void StateBox_PositionUpdate()
    {
        _boxPositionCount = 0;

        for (int i = 0; i < currentStateBoxes.Count; i++)
        {
            if (currentStateBoxes[i].isTransparent == false)
            {
                currentStateBoxes[i].transform.localPosition = new Vector2(0f, _boxPositionCount);
                _boxPositionCount += 0.3f;
            }
        }
    }

    // All States Transparency Toggle
    public void CurrentStates_Transparency(bool isTransparent)
    {
        for (int i = 0; i < currentStateBoxes.Count; i++)
        {
            currentStateBoxes[i].StateBox_Transparency(isTransparent);
        }
    }

    // Selected States Transparency Toggle
    public void CurrentStates_Transparency(FoodState_Type stateType, bool isTransparent)
    {
        FoodState_Box targetBox = FoodState_Box(stateType);

        if (targetBox != null)
        {
            targetBox.StateBox_Transparency(isTransparent);
        }

        StateBox_PositionUpdate();
    }

    // Current State Boxes Control
    public void Update_StateBox(FoodState_Type stateType, int level)
    {
        if (Has_State(stateType) == true)
        {
            FoodState_Box targetBox = FoodState_Box(stateType);

            targetBox.currentData.stateLevel += level;
            targetBox.Update_State();

            StateBox_PositionUpdate();
        }
        else
        {
            GameObject stateBox = Instantiate(_stateBox, _stateIndicator);

            stateBox.transform.localPosition = new Vector2(0f, _boxPositionCount);
            _boxPositionCount += 0.3f;

            if (stateBox.TryGetComponent(out FoodState_Box box))
            {
                currentStateBoxes.Add(box);
                box.Assign_StateController(this);
                box.Assign_State(stateType, level);
            }
        }
    }

    public void Clear_StateBox(FoodState_Type stateType)
    {
        if (Has_State(stateType) == true)
        {
            FoodState_Box(stateType).Clear_State();

            StateBox_PositionUpdate();
        }
    }
}
