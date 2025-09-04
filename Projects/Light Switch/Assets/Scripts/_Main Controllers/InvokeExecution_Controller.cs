using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvokeExecution_Controller : MonoBehaviour
{
    [SerializeField] [Range(0, 50)] private int _maxExecutionOrders;
    
    private List<Action> _executionActions = new();
    public List<Action> executionActions => _executionActions;


    // MonoBehaviour
    private void Start()
    {
        StartCoroutine(Invoke_Actions());
    }

    private void OnDestroy()
    {
        for (int i = 0; i < _executionActions.Count; i++)
        {
            _executionActions[i] = null;
        }
    }


    // Main
    private void Create_ExecutionAction(int executionOrder)
    {
        int executionCount = executionOrder + 1;
        
        for (int i = 0; i < executionCount; i++)
        {
            if (_executionActions.Count >= executionCount) return;
            _executionActions.Add(null);
        }
    }
    
    public void Subscribe_Action(Action action, int executionOrder)
    {
        int order = Mathf.Clamp(executionOrder, 0, _maxExecutionOrders - 1);
        
        Create_ExecutionAction(order);
        _executionActions[order] += action;
    }
    public void Unsubscribe_Action(Action action, int executionOrder)
    {
        int order = Mathf.Clamp(executionOrder, 0, _maxExecutionOrders - 1);
        _executionActions[order] -= action;
    }

    private IEnumerator Invoke_Actions()
    {
        yield return new WaitForEndOfFrame();
        
        for (int i = 0; i < _executionActions.Count; i++)
        {
            _executionActions[i]?.Invoke();
        }
    }
}
