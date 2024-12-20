using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSelector : MonoBehaviour
{
    private Action OnAction;


    public void Subscribe_Action(Action subscribeAction)
    {
        OnAction += subscribeAction;
    }

    public void Invoke_Action(int actionIndexNum)
    {
        if (OnAction == null) return;

        Delegate[] allActions = OnAction.GetInvocationList();

        if (actionIndexNum < 0 || actionIndexNum >= allActions.Length) return;

        allActions[actionIndexNum].DynamicInvoke();
    }
}
