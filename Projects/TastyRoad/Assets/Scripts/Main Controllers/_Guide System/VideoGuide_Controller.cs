using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

public class VideoGuide_Controller : MonoBehaviour, ISaveLoadable
{
    public static VideoGuide_Controller instance;


    [Space(20)]
    [SerializeField] private VideoPlayer _videoPlayer;
    
    [Space(20)]
    [SerializeField] private UI_EffectController _uiEffectController;
    [SerializeField] private Image _playerPanel;
    
    [Space(20)]
    [SerializeField] private TextMeshProUGUI _infoText;
    [SerializeField] private TextMeshProUGUI _navigateText;
    
    [Space(20)] 
    [SerializeField] private UI_ClockTimer _holdTimer;
    
    [Space(80)]
    [SerializeField] private Input_Manager _inputManager;


    private bool _guideActive;
    public bool guideActive => _guideActive;

    private bool _guideToggled;
    public bool guideToggled => _guideToggled;
    
    private List<int> _triggeredGuideIDs = new();

    private Guide_ScrObj _currentGuide;
    private int _currentClipNum;

    public Action OnGuide_ActivationTrigger;
    public Action OnGuideTrigger;
    public Action OnGuideToggle;


    // UnityEngine
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Input_Controller input = Input_Controller.instance;
        
        input.Update_EmojiAsset(_infoText);
        input.Update_EmojiAsset(_navigateText);

        // subscriptions
        _inputManager.OnSelectStart += _holdTimer.Run_ClockSprite;
        _inputManager.OnSelect += _holdTimer.Stop_ClockSpriteRun;
        _inputManager.OnHoldSelect += _holdTimer.Stop_ClockSpriteRun;

        _inputManager.OnHoldSelect += () => Toggle_GuideActivation(false);
        
        _inputManager.OnSelect += Navigate_NextVideo;
        _inputManager.OnExit += Navigate_NextVideo;
        
        input.OnSchemeUpdate += () => input.Update_EmojiAsset(_infoText);
        input.OnSchemeUpdate += () => input.Update_EmojiAsset(_navigateText);

        Localization_Controller.instance.OnLanguageChanged += () => Update_ClipData(_currentClipNum);
        Localization_Controller.instance.OnLanguageChanged += Update_NavigateText;
    }
    
    private void OnDestroy()
    {
        // subscriptions
        _inputManager.OnSelectStart -= _holdTimer.Run_ClockSprite;
        _inputManager.OnSelect -= _holdTimer.Stop_ClockSpriteRun;
        _inputManager.OnHoldSelect -= _holdTimer.Stop_ClockSpriteRun;
        
        _inputManager.OnHoldSelect -= () => Toggle_GuideActivation(false);
        
        _inputManager.OnSelect -= Navigate_NextVideo;
        _inputManager.OnExit -= Navigate_NextVideo;
        
        Input_Controller input = Input_Controller.instance;
        
        input.OnSchemeUpdate -= () => input.Update_EmojiAsset(_infoText);
        input.OnSchemeUpdate -= () => input.Update_EmojiAsset(_navigateText);
        
        Localization_Controller.instance.OnLanguageChanged -= () => Update_ClipData(_currentClipNum);
        Localization_Controller.instance.OnLanguageChanged -= Update_NavigateText;
    }


    // ISaveLoadable
    public void Save_Data()
    {
        ES3.Save("VideoGuide_Controller/Guide_ActiveState", _guideActive);
        ES3.Save("VideoGuide_Controller/Triggered_GuideIDs", _triggeredGuideIDs);
    }

    public void Load_Data()
    {
        _triggeredGuideIDs = ES3.Load("VideoGuide_Controller/Triggered_GuideIDs", _triggeredGuideIDs);

        if (ES3.KeyExists("VideoGuide_Controller/Guide_ActiveState"))
        {
            _guideActive = ES3.Load("VideoGuide_Controller/Guide_ActiveState", _guideActive);
            return;
        }
        _guideActive = true;
    }
    
    
    // Data
    private void Toggle_GuideActivation(bool toggle)
    {
        _guideActive = toggle;
        OnGuide_ActivationTrigger?.Invoke();

        if (toggle) return;
        Toggle_VideoPanel(false);
    }
    
    
    // Trigger
    public bool Guide_Triggered(Guide_ScrObj guideScrObj)
    {
        for (int i = 0; i < _triggeredGuideIDs.Count; i++)
        {
            if (guideScrObj.guideID != _triggeredGuideIDs[i]) continue;
            return true;
        }
        return false;
    }

    public void Trigger_Guide(Guide_ScrObj guideScrObj)
    {
        if (_guideActive == false) return;
        if (guideScrObj == null) return;
        if (Guide_Triggered(guideScrObj)) return;
        
        _triggeredGuideIDs.Add(guideScrObj.guideID);
        
        if (guideScrObj.clipDatas.Length <= 0) return;
        _currentGuide = guideScrObj;
        
        Toggle_VideoPanel(true);
        OnGuideTrigger?.Invoke();
    }
    public void Trigger_Guide(VideoGuide_Trigger guideTrigger)
    {
        if (guideTrigger == null) return;
        
        Trigger_Guide(guideTrigger.triggerGuide);
    }
    

    // UI Control
    private void Update_ClipData(int clipNum)
    {
        if (_currentGuide == null) return;
        
        VideoClip_Data clipData = _currentGuide.clipDatas[clipNum];
        
        Toggle_VideoClip(clipData.video);
        _infoText.text = clipData.Info();
    }
    
    private void Update_NavigateText()
    {
        if (_currentGuide == null) return;
        
        InfoTemplate_Trigger infoTrigger = gameObject.GetComponent<InfoTemplate_Trigger>();

        if (_currentClipNum >= _currentGuide.clipDatas.Length - 1)
        {
            _navigateText.text = infoTrigger.TemplateString(1);
            return;
        }
        
        _navigateText.text = infoTrigger.TemplateString(0);
    }
    
    
    private void Toggle_VideoClip(VideoClip clip)
    {
        if (clip == null) return;

        _videoPlayer.clip = clip;
        _videoPlayer.Play();
    }

    private void Toggle_VideoPanel(bool toggle)
    {
        Input_Controller input = Input_Controller.instance;

        _guideToggled = toggle;
        Main_Controller.instance.transitionCanvas.Toggle_PauseScreen(toggle);
        _playerPanel.gameObject.SetActive(toggle);
        
        OnGuideToggle?.Invoke();

        if (toggle == false)
        {
            _currentGuide = null;
            _inputManager.Toggle_Input(false);

            _videoPlayer.Stop();
            return;
        }

        _currentClipNum = 0;
        
        _inputManager.Toggle_Input(true);
        _uiEffectController.Update_Scale(_playerPanel.rectTransform);

        VideoClip_Data clipData = _currentGuide.clipDatas[_currentClipNum];

        Toggle_VideoClip(clipData.video);
        _infoText.text = clipData.Info();
        
        Update_NavigateText();
    }


    public void Navigate_NextVideo()
    {
        _currentClipNum++;

        if (_currentClipNum > _currentGuide.clipDatas.Length - 1)
        {
            Toggle_VideoPanel(false);
            return;
        }
        
        Update_ClipData(_currentClipNum);
        Update_NavigateText();
    }
}
