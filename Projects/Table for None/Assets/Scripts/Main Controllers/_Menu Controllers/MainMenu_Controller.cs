using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using FMOD.Studio;

public class MainMenu_Controller : Menu_Controller
{
    [Space(20)]
    [SerializeField] private InfoTemplate_Trigger _infoTemplate;
    [SerializeField] private TextMeshProUGUI _dataInfoText;
    
    [Space(20)]
    [SerializeField] private Menu_EventButton[] _linkButtons;
    
    
    // UnityEngine
    private new void Start()
    {
        Set_CurrentIndex(eventButtons.Length);
        base.Start();
        
        Toggle_Menu(true);
        StartCoroutine(Play_ThemeBGM());

        foreach (Menu_EventButton button in _linkButtons)
        {
            button.Set_DefaultPosition();
        }
        
        Input_Controller.instance.Toggle_Input(true);
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
    
    
    // Start Game Panel
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

    public void Quit_Game()
    {
        if (Input_Controller.instance.currentScheme.name != "PC") return;
        
        Application.Quit();
        Debug.Log("Application Quit Successful from: " + Input_Controller.instance.playerInput.currentControlScheme);
    }


    public void Reset_Data()
    {
        if (ES3.FileExists("SaveFile.es3") == false) return;
        
        ES3.DeleteFile("SaveFile.es3");
    }
    
    public void Update_DataInfoText(bool newData)
    {
        if (newData)
        {
            _dataInfoText.text = _infoTemplate.TemplateString(1);
            return;
        }
        
        if (ES3.FileExists("SaveFile.es3") == false)
        {
            _dataInfoText.text = _infoTemplate.TemplateString(0);
            return;
        }
        
        // get current location data
        WorldMap_Data mapData = ES3.Load<WorldMap_ControllerData>("WorldMap_Controller/WorldMap_ControllerData").currentData;
        string mapDataString = "<sprite=95> " + (mapData.worldNum) + "-" + (mapData.locationNum);

        // get current gold amount
        GoldSystem_Data goldData = ES3.Load<GoldSystem_Data>("GoldSystem/GoldSystem_Data");
        string goldDataString = "<sprite=56> " + goldData.goldAmount;
        
        _dataInfoText.text = mapDataString + "   " + goldDataString;
    }
    public void Update_DataInfoText()
    {
        _dataInfoText.text = _infoTemplate.TemplateString(0);
    }
    
    
    // Link buttons
    public void Hover_LinkButton(int buttonIndex)
    {
        if (buttonIndex < 0)
        {
            foreach (Menu_EventButton button in _linkButtons)
            {
                RectTransform buttonTransform = button.button.rectTransform;
                buttonTransform.localPosition = button.defaultPosition;
            }
            return;
        }
        
        for (int i = 0; i < _linkButtons.Length; i++)
        {
            RectTransform buttonTransform = _linkButtons[i].button.rectTransform;
            
            if (i != buttonIndex)
            {
                buttonTransform.localPosition = _linkButtons[i].defaultPosition;
                continue;
            }
            
            buttonTransform.localPosition = Vector2.zero;
        }
    }

    public void Open_Link(string link)
    {
        Application.OpenURL(link);
    }
}