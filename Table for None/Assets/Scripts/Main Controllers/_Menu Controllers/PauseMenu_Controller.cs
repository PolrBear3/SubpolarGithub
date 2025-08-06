using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu_Controller : Menu_Controller
{
    public static PauseMenu_Controller instance;

    
    [Space(20)] 
    [SerializeField] private UI_EffectController _effectController;
    [SerializeField] private Sprite _mainMenuIcon;

    [Space(20)] 
    [SerializeField] private UI_ClockTimer _guideToggleTimer;
    [SerializeField] private InfoTemplate_Trigger _guideToggleTrigger;
    [SerializeField] private TextMeshProUGUI _guideToggleText;


    private bool _isPaused;
    public bool isPaused => _isPaused;
    
    private Action OnPauseTrue;
    private Action OnPauseFalse;
    
    public Action OnPause;
    public Action OnPauseExit;


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
        OnPauseTrue = () => Toggle_Pause(true);
        OnPauseFalse = () => Toggle_Pause(false);
        
        Input_Controller.instance.OnCancel += OnPauseTrue;
        inputManager.OnExit += OnPauseFalse;
        
        inputManager.OnSelectStart += _guideToggleTimer.Run_ClockSprite;
        inputManager.OnSelect += _guideToggleTimer.Stop_ClockSpriteRun;
        inputManager.OnHoldSelect += _guideToggleTimer.Stop_ClockSpriteRun;

        VideoGuide_Controller videoGuideController = VideoGuide_Controller.instance;
        
        inputManager.OnHoldSelect += VideoGuide_Controller.instance.Toggle_GuideActivation;
        videoGuideController.OnGuide_ActivationTrigger += Update_GuideToggleText;
        
        Localization_Controller.instance.OnLanguageChanged += Update_GuideToggleText;
    }

    private new void OnDestroy()
    {
        // subscriptions
        Input_Controller.instance.OnCancel -= OnPauseTrue;
        inputManager.OnExit -= OnPauseFalse;
        
        inputManager.OnSelectStart -= _guideToggleTimer.Run_ClockSprite;
        inputManager.OnSelect -= _guideToggleTimer.Stop_ClockSpriteRun;
        inputManager.OnHoldSelect -= _guideToggleTimer.Stop_ClockSpriteRun;
        
        VideoGuide_Controller videoGuideController = VideoGuide_Controller.instance;
        
        inputManager.OnHoldSelect -= videoGuideController.Toggle_GuideActivation;
        videoGuideController.OnGuide_ActivationTrigger -= Update_GuideToggleText;
        
        Localization_Controller.instance.OnLanguageChanged -= Update_GuideToggleText;
    }


    // Pause Options
    public void Toggle_Pause(bool toggle)
    {
        if (Input_Controller.instance.isHolding) return;
        
        TransitionCanvas_Controller transition = TransitionCanvas_Controller.instance;
        if (transition.coroutine != null) return;

        _isPaused = toggle;
        
        Toggle_Menu(toggle);
        transition.Toggle_PauseScreen(toggle);

        if (toggle == false)
        {
            OnExitMenu?.Invoke();
            OnPauseExit?.Invoke();

            Audio_Controller.instance.Play_OneShot(gameObject, 0);
            return;
        }

        Update_GuideToggleText();
        
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
    
    
    // Video Guide System
    private void Update_GuideToggleText()
    {
        bool guideActive = VideoGuide_Controller.instance.guideActive;
        
        _guideToggleText.text = guideActive ? _guideToggleTrigger.TemplateString(1) : _guideToggleTrigger.TemplateString(0);
    }
}
