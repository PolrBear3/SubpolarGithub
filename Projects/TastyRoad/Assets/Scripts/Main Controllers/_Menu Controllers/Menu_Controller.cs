using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Menu_Controller : MonoBehaviour
{
    [Header("")]
    [SerializeField] private UnityEvent[] _selectActions;

    private int _currentIndex;
    public int currentIndex => _currentIndex;

    public Action OnNavigate;
    public Action OnAction;


    // MonoBehaviour
    public void Awake()
    {

    }

    public void Start()
    {

    }

    public void OnDestroy()
    {
        Input_Controller input = Input_Controller.instance;

        input.OnCursorControl -= Navigate_Action;
        input.OnSelect -= Select_Action;
    }


    // Data Control
    public void Set_CurrentIndex(int indexNum)
    {
        _currentIndex = Mathf.Clamp(indexNum, 0, _selectActions.Length - 1);
    }


    // Menu Control
    public void Toggle_InputControl(bool toggle)
    {
        Input_Controller input = Input_Controller.instance;

        if (toggle)
        {
            input.Update_ActionMap(1);

            input.OnCursorControl += Navigate_Action;
            input.OnSelect += Select_Action;

            return;
        }

        input.Update_ActionMap(0);

        input.OnCursorControl -= Navigate_Action;
        input.OnSelect -= Select_Action;
    }


    private void Navigate_Action(Vector2 direction)
    {
        int actionCount = _selectActions.Length;
        if (actionCount <= 0) return;

        float value = direction.x + direction.y;
        _currentIndex = (_currentIndex + (int)value + actionCount) % actionCount;

        OnNavigate?.Invoke();
    }

    private void Select_Action()
    {
        if (_selectActions.Length <= 0) return;

        _selectActions[_currentIndex]?.Invoke();
        OnAction?.Invoke();
    }
}
