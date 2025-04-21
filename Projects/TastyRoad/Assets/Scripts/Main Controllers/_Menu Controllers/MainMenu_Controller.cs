using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMOD.Studio;

public class MainMenu_Controller : Menu_Controller
{
    // UnityEngine
    private new void Start()
    {
        Set_CurrentIndex(eventButtons.Length);
        base.Start();
        
        Toggle_Menu(true);
        StartCoroutine(Play_ThemeBGM());
    }
    
    
    // Menu Options
    private IEnumerator Play_ThemeBGM()
    {
        while (TransitionCanvas_Controller.instance.coroutine != null) yield return null;
        
        EventInstance instance = Audio_Controller.instance.Create_EventInstance(gameObject, 2);

        instance.setParameterByName("Value_intensity", 1f);
        instance.start();
        
        yield break;
    }
    
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
        gameObject.GetComponent<SoundData_Controller>().FadeOut(2);

        while (transition.coroutine != null) yield return null;

        SceneManager.LoadScene(1);
        yield break;
    }
}
