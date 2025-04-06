using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu_Controller : Menu_Controller
{
    public static PauseMenu_Controller instance;


    [Header("")]
    [SerializeField] private GameObject _menuPanel;
    [SerializeField] private RectTransform[] _selectBoxes;


    private Vector2 _unSelectPosition;

    public Action OnPause;


    // Menu_Controller
    public new void Awake()
    {
        instance = this;
    }

    public new void Start()
    {
        Set_CurrentIndex(_selectBoxes.Length);

        _unSelectPosition = new(_selectBoxes[0].anchoredPosition.x, _selectBoxes[0].anchoredPosition.y);

        _menuPanel.SetActive(false);

        // subscriptions
        Input_Controller input = Input_Controller.instance;

        input.OnCancel += Toggle_Pause;
        input.OnExit += Toggle_Pause;

        OnNavigate += Navigate_SelectBox;
    }

    public new void OnDestroy()
    {
        // subscriptions
        Input_Controller input = Input_Controller.instance;

        input.OnCancel -= Toggle_Pause;
        input.OnExit -= Toggle_Pause;

        OnNavigate -= Navigate_SelectBox;
    }


    // Pause Options
    public void Toggle_Pause()
    {
        if (TransitionCanvas_Controller.transitionPlaying) return;

        Input_Controller input = Input_Controller.instance;

        if (_menuPanel.activeSelf == false && input.Current_ActionMapNum() == 1) return;

        _menuPanel.SetActive(!_menuPanel.activeSelf);

        Main_Controller.instance.transitionCanvas.Toggle_PauseScreen(_menuPanel.activeSelf);
        Toggle_InputControl(_menuPanel.activeSelf);

        if (_menuPanel.activeSelf == false) return;

        OnPause?.Invoke();
        Navigate_SelectBox();
    }

    private void Navigate_SelectBox()
    {
        for (int i = 0; i < _selectBoxes.Length; i++)
        {
            if (i != currentIndex)
            {
                _selectBoxes[i].localPosition = _unSelectPosition;
                continue;
            }

            _selectBoxes[i].localPosition = Vector2.zero;
        }
    }
}
