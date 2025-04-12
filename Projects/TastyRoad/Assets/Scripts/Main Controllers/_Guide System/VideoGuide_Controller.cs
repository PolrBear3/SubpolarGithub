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
    [SerializeField] private TextMeshProUGUI _continueText;

    [Header("")]
    [SerializeField] private Guide_ScrObj[] _allGuides;


    private List<GuideData> _triggeredDatas = new();

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
        input.Update_EmojiAsset(_continueText);

        // subscriptions
        input.OnSchemeUpdate += () => input.Update_EmojiAsset(_infoText);
        input.OnSchemeUpdate += () => input.Update_EmojiAsset(_continueText);
    }
    
    private void OnDestroy()
    {
        // subscriptions
        Input_Controller input = Input_Controller.instance;

        input.OnSelect -= Navigate_NextVideo;
        input.OnExit -= Navigate_NextVideo;
        
        input.OnSchemeUpdate -= () => input.Update_EmojiAsset(_infoText);
        input.OnSchemeUpdate -= () => input.Update_EmojiAsset(_continueText);
    }


    // ISaveLoadable
    public void Save_Data()
    {
        ES3.Save("VideoGuide_Controller/_triggeredDatas", _triggeredDatas);
    }

    public void Load_Data()
    {
        _triggeredDatas = ES3.Load("VideoGuide_Controller/_triggeredDatas", _triggeredDatas);
    }


    // Trigger
    private Guide_ScrObj Guide(string guideName)
    {
        for (int i = 0; i < _allGuides.Length; i++)
        {
            if (guideName != _allGuides[i].guideName) continue;

            return _allGuides[i];
        }
        return null;
    }

    private bool Guide_Triggered(string guideName)
    {
        for (int i = 0; i < _triggeredDatas.Count; i++)
        {
            if (guideName != _triggeredDatas[i].guideScrObj.guideName) continue;

            return true;
        }
        return false;
    }


    public void Trigger_Guide(string guideName)
    {
        if (Guide_Triggered(guideName)) return;

        Guide_ScrObj guide = Guide(guideName);
        if (guide == null) return;

        _triggeredDatas.Add(new(guide));
        _currentGuide = guide;

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
        _infoText.text = clipData.info;
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
        VideoClip_Data clipData = _currentGuide.clipDatas[_currentClipNum];

        Toggle_VideoClip(clipData.video);
        _infoText.text = clipData.info;
    }
}
