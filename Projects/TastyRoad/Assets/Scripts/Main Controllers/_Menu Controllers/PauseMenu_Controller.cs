using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu_Controller : Menu_Controller
{
    [Header("")]
    [SerializeField] private GameObject _menuPanel;


    // Menu_Controller
    public new void Start()
    {
        _menuPanel.SetActive(false);

        // subscriptions
        Input_Controller input = Input_Controller.instance;

        input.OnCancel += Toggle_Pause;
        input.OnExit += Toggle_Pause;
    }

    public new void OnDestroy()
    {
        // subscriptions
        Input_Controller input = Input_Controller.instance;

        input.OnCancel -= Toggle_Pause;
        input.OnExit -= Toggle_Pause;
    }


    // Pause Options
    public void Toggle_Pause()
    {
        Input_Controller input = Input_Controller.instance;

        if (_menuPanel.activeSelf == false && input.Current_ActionMapNum() == 1) return;

        _menuPanel.SetActive(!_menuPanel.activeSelf);

        Main_Controller.instance.transitionCanvas.Toggle_PauseScreen(_menuPanel.activeSelf);
        Toggle_InputControl(_menuPanel.activeSelf);
    }
}
