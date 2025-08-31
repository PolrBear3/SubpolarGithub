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
    [SerializeField] private Image _prepareBox;
    
    [Space(20)]
    [SerializeField] private TextMeshProUGUI _infoText;
    [SerializeField] private TextMeshProUGUI _navigateText;

    [Space(20)] 
    [SerializeField] private GameObject _guideToggleBox;
    public GameObject guideToggleBox => _guideToggleBox;
    
    [SerializeField] private UI_ClockTimer _holdTimer;
    
    [Space(60)]
    [SerializeField] private Guide_ScrObj _triggerGuide;
    [SerializeField] private Input_Manager _inputManager;


    private VideoGuide_ControllerData _data;
    public VideoGuide_ControllerData data => _data;

    private bool _guideToggled;
    public bool guideToggled => _guideToggled;

    private Guide_ScrObj _currentGuide;
    private int _currentClipNum;

    public Action OnGuide_ActivationTrigger;
    public Action OnGuideTrigger;
    public Action OnGuideToggle;
    
    private Coroutine _videoCoroutine;


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
        Cutscene_Controller.instance.OnEnd += Load_GuideActivation;
        
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
        Cutscene_Controller.instance.OnEnd -= Load_GuideActivation;
        
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
        ES3.Save("VideoGuide_Controller/VideoGuide_ControllerData", _data);
    }

    public void Load_Data()
    {
        _data = ES3.Load("VideoGuide_Controller/VideoGuide_ControllerData", new VideoGuide_ControllerData(true));
    }
    
    
    // Data
    public void Toggle_GuideActivation(bool toggle)
    {
        if (PauseMenu_Controller.instance.isPaused) return;
        
        _data.Toggle_GuideActivation(toggle);
        OnGuide_ActivationTrigger?.Invoke();

        if (toggle) return;
        if (_playerPanel.gameObject.activeSelf == false) return;
        
        Toggle_VideoPanel(false);
    }
    public void Toggle_GuideActivation()
    {
        _data.Toggle_GuideActivation(!_data.guideActive);
        OnGuide_ActivationTrigger?.Invoke();
    }

    private void Load_GuideActivation()
    {
        bool guideActive = ES3.Load("VideoGuide_Controller/VideoGuide_ControllerData", new VideoGuide_ControllerData(true)).guideActive;
        if (guideActive == false) return;
        
        Toggle_GuideActivation(true);
        VideoGuide_Controller.instance.Trigger_Guide(_triggerGuide);
    }
    
    
    // Trigger
    public bool Guide_Triggered(Guide_ScrObj guideScrObj)
    {
        return _data.triggeredGuides.Contains(guideScrObj);
    }
    
    public void Trigger_Guide(Guide_ScrObj guideScrObj)
    {
        if (_guideToggled) return;
        if (_data.guideActive == false) return;
        if (guideScrObj == null) return;
        if (Guide_Triggered(guideScrObj)) return;
        
        _data.triggeredGuides.Add(guideScrObj);
        
        if (guideScrObj.clipDatas.Length <= 0) return;
        _currentGuide = guideScrObj;
        
        Toggle_VideoPanel(true);
        OnGuideTrigger?.Invoke();
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

        if (_videoCoroutine != null) StopCoroutine(_videoCoroutine);
        _videoCoroutine = StartCoroutine(VideoClip_Coroutine(clip));
    }
    private IEnumerator VideoClip_Coroutine(VideoClip clip)
    {
        _videoPlayer.Stop();
        
        _prepareBox.color = Color.black;
        
        _videoPlayer.clip = clip;
        _videoPlayer.Prepare();
        
        while (!_videoPlayer.isPrepared) yield return null;

        LeanTween.alpha(_prepareBox.rectTransform, 0f, 0.1f);
        
        _videoPlayer.time = 0;
        _videoPlayer.frame = 0;
        _videoPlayer.Play();

        _videoCoroutine = null;
    }

    private void Toggle_VideoPanel(bool toggle)
    {
        _guideToggled = toggle;

        bool showPauseScreen = toggle || PauseMenu_Controller.instance.isPaused;
        Main_Controller.instance.transitionCanvas.Toggle_PauseScreen(showPauseScreen);

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
    public void Toggle_VideoPanel(Guide_ScrObj guideScrObj)
    {
        _currentGuide = guideScrObj;
        _guideToggled = true;
        _currentClipNum = 0;
        
        Main_Controller.instance.transitionCanvas.Toggle_PauseScreen(true);
        _playerPanel.gameObject.SetActive(true);
        
        OnGuideToggle?.Invoke();
        
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
        
        // sound
        Audio_Controller.instance.Play_OneShot(gameObject, 1);
    }
}
