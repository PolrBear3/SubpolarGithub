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

        input.OnCancel += () => Toggle_Pause(true);
    }

    private new void OnDestroy()
    {
        // subscriptions
        Input_Controller input = Input_Controller.instance;

        input.OnCancel -= () => Toggle_Pause(true);
    }


    // Pause Options
    public void Toggle_Pause(bool toggle)
    {
        TransitionCanvas_Controller transition = TransitionCanvas_Controller.instance;
        if (transition.coroutine != null) return;
        
        Input_Controller input = Input_Controller.instance;
        if (Menu_Toggled() == false && input.Current_ActionMapNum() == 1) return;

        Toggle_Menu(toggle);
        transition.Toggle_PauseScreen(toggle);

        if (toggle == false)
        {
            OnExitMenu?.Invoke();

            Audio_Controller.instance.Play_OneShot(gameObject, 0);
            return;
        }

        OnPause?.Invoke();
    }
    
    
    public void Return_MainMenu()
    {
        SaveLoad_Controller.instance.SaveAll_ISaveLoadable();
        
        StartCoroutine(Return_MainMenu_Coroutine());
    }
    private IEnumerator Return_MainMenu_Coroutine()
    {
        Main_Controller.instance.Player().Toggle_Controllers(false);
        
        TransitionCanvas_Controller transition = TransitionCanvas_Controller.instance;
        
        transition.Set_LoadIcon(_mainMenuIcon);
        transition.CloseScene_Transition();
        
        Audio_Controller.instance.Play_OneShot(gameObject, 0);

        while (transition.coroutine != null) yield return null;

        SceneManager.LoadScene(0);
        yield break;
    }
}
