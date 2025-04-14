using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu_Controller : Menu_Controller
{
    public static PauseMenu_Controller instance;

    public Action OnPause;


    // Menu_Controller
    public new void Awake()
    {
        instance = this;
    }

    public new void Start()
    {
        base.Start();
        
        menuPanel.gameObject.SetActive(false);

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
        TransitionCanvas_Controller transition = TransitionCanvas_Controller.instance;
        
        if (transition.transitionPlaying) return;

        Input_Controller input = Input_Controller.instance;

        if (Menu_Toggled() == false && input.Current_ActionMapNum() == 1) return;

        Toggle_Menu(!Menu_Toggled());
        transition.Toggle_PauseScreen(Menu_Toggled());

        if (Menu_Toggled() == false) return;
        OnPause?.Invoke();
        
        // sound
        Audio_Controller.instance.Play_OneShot(gameObject, 0);
    }
}
