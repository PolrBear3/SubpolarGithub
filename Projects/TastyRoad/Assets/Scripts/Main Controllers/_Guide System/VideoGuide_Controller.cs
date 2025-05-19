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


    [Header("")]
    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private Image _playerPanel;
    
    [Header("")]
    [SerializeField] private TextMeshProUGUI _infoText;
    [SerializeField] private TextMeshProUGUI _navigateText;

    
    [Space(80)]
    [SerializeField] private Input_Manager _inputManager;


    private List<int> _triggeredGuideNums = new();

    private Guide_ScrObj _currentGuide;
    private int _currentClipNum;

    public Action OnGuideTrigger;


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
        ES3.Save("VideoGuide_Controller/_triggeredGuideNums", _triggeredGuideNums);
    }

    public void Load_Data()
    {
        _triggeredGuideNums = ES3.Load("VideoGuide_Controller/_triggeredGuideNums", _triggeredGuideNums);
    }
    
    
    // Trigger
    public bool Guide_Triggered(Guide_ScrObj guideScrObj)
    {
        for (int i = 0; i < _triggeredGuideNums.Count; i++)
        {
            if (guideScrObj.guideID != _triggeredGuideNums[i]) continue;
            return true;
        }
        return false;
    }


    public void Trigger_Guide(Guide_ScrObj guideScrObj)
    {
        if (guideScrObj == null) return;
        if (Guide_Triggered(guideScrObj)) return;
        
        _triggeredGuideNums.Add(guideScrObj.guideID);
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
    private void Toggle_VideoClip(VideoClip clip)
    {
        if (clip == null) return;

        _videoPlayer.clip = clip;
        _videoPlayer.Play();
    }

    private void Toggle_VideoPanel(bool toggle)
    {
        Input_Controller input = Input_Controller.instance;

        Main_Controller.instance.transitionCanvas.Toggle_PauseScreen(toggle);
        _playerPanel.gameObject.SetActive(toggle);

        if (toggle == false)
        {
            _currentGuide = null;
            _inputManager.Toggle_Input(false);

            _videoPlayer.Stop();
            return;
        }

        _currentClipNum = 0;
        _inputManager.Toggle_Input(true);

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
}
