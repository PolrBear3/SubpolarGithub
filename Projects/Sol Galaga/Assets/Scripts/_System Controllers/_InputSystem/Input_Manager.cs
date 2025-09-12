using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Input_Manager : MonoBehaviour
{
    public Action<Vector2> OnCursorControl;

    public Action OnSelectStart;
    public Action OnSelect;
    public Action OnHoldSelect;

    public Action OnOption1;
    public Action OnOption2;

    public Action OnExit;
    
    
    // Input Toggle Control
    private void Update_ActionMap()
    {
        Input_Controller inputController = Input_Controller.instance;

        if (inputController.activeInputManagers.Count > 0) return;
        inputController.Update_ActionMap(0);
    }
    
    public void Toggle_Input(bool toggle)
    {
        Input_Controller inputController = Input_Controller.instance;
        
        if (toggle && inputController.activeInputManagers.Contains(this) == false)
        {
            inputController.activeInputManagers.Add(this);
            inputController.Update_ActionMap(1);
            
            return;
        }
        
        inputController.activeInputManagers.Remove(this);
        Update_ActionMap();
    }
}