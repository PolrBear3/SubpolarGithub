using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu_Controller : Menu_Controller
{
    // UnityEngine
    private new void Start()
    {
        Set_CurrentIndex(eventButtons.Length);
        base.Start();
        
        Toggle_Menu(true);
    }
    
    
    // Menu Options
    public void Start_InGame()
    {
        StartCoroutine(Start_InGame_Coroutine());
    }
    private IEnumerator Start_InGame_Coroutine()
    {
        // lock player input //
        Input_Controller.instance.Update_ActionMap(0);
        
        TransitionCanvas_Controller transition = TransitionCanvas_Controller.instance;
        transition.CloseScene_Transition();
        
        Audio_Controller.instance.Play_OneShot(gameObject, 0);

        while (transition.coroutine != null) yield return null;

        SceneManager.LoadScene(1);
        yield break;
    }
}
