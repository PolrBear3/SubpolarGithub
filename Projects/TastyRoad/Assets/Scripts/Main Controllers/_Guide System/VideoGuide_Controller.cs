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

    [Header("")]
    [SerializeField] private Guide_ScrObj[] _allGuides;


    private List<int> _triggeredGuideNums = new();

    private Guide_ScrObj _currentGuide;
    private int _currentClipNum;


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
        input.OnSchemeUpdate += () => input.Update_EmojiAsset(_infoText);
        input.OnSchemeUpdate += () => input.Update_EmojiAsset(_navigateText);

        Localization_Controller.instance.OnLanguageChanged += () => Update_ClipData(_currentClipNum);
        Localization_Controller.instance.OnLanguageChanged += Update_NavigateText;
    }
    
    private void OnDestroy()
    {
        // subscriptions
        Input_Controller input = Input_Controller.instance;

        input.OnSelect -= Navigate_NextVideo;
        input.OnExit -= Navigate_NextVideo;
        
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
    private bool Guide_Triggered(Guide_ScrObj guideScrObj)
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
            
            input.OnSelect -= Navigate_NextVideo;
            input.OnExit -= Navigate_NextVideo;
            input.Update_ActionMap(0);

            _videoPlayer.Stop();
            return;
        }

        input.OnSelect += Navigate_NextVideo;
        input.OnExit += Navigate_NextVideo;
        input.Update_ActionMap(1);

        _currentClipNum = 0;
        VideoClip_Data clipData = _currentGuide.clipDatas[_currentClipNum];

        Toggle_VideoClip(clipData.video);
        _infoText.text = clipData.Info();
    }


    private void Navigate_NextVideo()
    {
        int nextClipNum = _currentClipNum + 1;

        if (nextClipNum > _currentGuide.clipDatas.Length - 1)
        {
            Toggle_VideoPanel(false);
            return;
        }

        _currentClipNum = nextClipNum;
        
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
        int nextClipNum = _currentClipNum + 1;
        
        if (nextClipNum > _currentGuide.clipDatas.Length - 1)
        {
            _navigateText.text = infoTrigger.TemplateString(1);
            return;
        }
        
        _navigateText.text = infoTrigger.TemplateString(0);
    }
}
