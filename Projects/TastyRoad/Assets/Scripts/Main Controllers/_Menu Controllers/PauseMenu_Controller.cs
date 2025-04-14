using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu_Controller : Menu_Controller
{
    public static PauseMenu_Controller instance;

    
    [Header("")] 
    [SerializeField] private Sprite _mainMenuIcon;
    
    public Action OnPause;


    // Menu_Controller
    private new void Awake()
    {
        instance = this;
    }
    
    private new void Start()
    {
        Set_CurrentIndex(eventButtons.Length);
        base.Start();

        // subscriptions
        Input_Controller input = Input_Controller.instance;

        input.OnCancel += Toggle_Pause;
        input.OnExit += Toggle_Pause;
    }

    private new void OnDestroy()
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

        if (Menu_Toggled() == false)
        {
            Audio_Controller.instance.Play_OneShot(gameObject, 0);
            return;
        }
        
        OnPause?.Invoke();
    }

    public void Return_MainMenu()
    {
        StartCoroutine(Return_MainMenu_Coroutine());
    }
    private IEnumerator Return_MainMenu_Coroutine()
    {
        // lock player input //
        
        TransitionCanvas_Controller transition = TransitionCanvas_Controller.instance;
        
        transition.Set_LoadIcon(_mainMenuIcon);
        transition.CloseScene_Transition();
        
        Audio_Controller.instance.Play_OneShot(gameObject, 0);

        while (transition.transitionPlaying) yield return null;

        SceneManager.LoadScene(0);
        yield break;
    }
}
