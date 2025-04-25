using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using FMODUnity;
using FMOD.Studio;

[System.Serializable]
public struct ResolutionData
{
    [SerializeField] private int _width;
    public int width => _width;
    
    [SerializeField] private int _height;
    public int height => _height;
}

public class OptionsMenu_Controller : Menu_Controller
{
    [Header("")]
    [SerializeField] private TextMeshProUGUI[] _adjustmentTexts;

    [SerializeField] private ResolutionData[] _fallBackResolutions;


    private bool _adjusting;
    
    private List<string> _optionTexts = new();
    private int _selectedTextIndex;
    
    private Action<Vector2> OnOptionNavigate;
    private int _indexNum;

    private OptionsData _preData;
    private OptionsData _data;
    
    private List<Vector2Int> _resolutions = new();
    private Coroutine _resCoroutine;


    // UnityEngine
    private new void Start()
    {
        _data = ES3.Load("OptionsMenu_Controller/OptionsData", new OptionsData(1f));
        Apply_OptionsData(_data);
        
        Set_CurrentIndex(eventButtons.Length);
        base.Start();

        Toggle_Menu(false);

        // save current option texts
        foreach (TextMeshProUGUI tmp in _adjustmentTexts)
        {
            _optionTexts.Add(tmp.text);
        }
    }
    
    
    // Options Control
    private void Apply_OptionsData(OptionsData data)
    {
        if (data == null) return;
        
        RuntimeManager.GetBus("bus:/").setVolume(data.volume);
        Screen.SetResolution(data.resolution.x, data.resolution.y, data.isFullScreen);
        Localization_Controller.instance.Update_Language(data.language);
    }
    
    public void Save_OptionsData()
    {
        /*
        if (_preData == null || _preData != _data)
        {
            // return to previous volume //
            _preData = null;
            
            return;
        }
        */
        
        // _data = new(_preData); //
        ES3.Save("OptionsMenu_Controller/OptionsData", _data);

        Screen.SetResolution(_data.resolution.x, _data.resolution.y, _data.isFullScreen);
        Localization_Controller.instance.Update_Language(_data.language);

        _preData = null;
    }
    
    
    private void Refresh_AdjustmentTexts()
    {
        for (int i = 0; i < _adjustmentTexts.Length; i++)
        {
            _adjustmentTexts[i].text = _optionTexts[i];
        }
    }
    
    
    public void Toggle_Adjustments(bool toggle)
    {
        if (_resCoroutine != null) return;
        
        _adjusting = toggle;
        
        Input_Controller input = Input_Controller.instance;

        if (toggle)
        {
            input.OnCursorControl += OnOptionNavigate;
            input.OnCursorControl += Exit_Adjustments;
            return;
        }

        input.OnCursorControl -= OnOptionNavigate;
        input.OnCursorControl -= Exit_Adjustments;
        OnOptionNavigate = null;

        Refresh_AdjustmentTexts();
        Save_OptionsData();
    }
    public void Toggle_Adjustments()
    {
        Toggle_Adjustments(!_adjusting);
    }
    
    public void Exit_Adjustments(Vector2 adjustDirection)
    {
        if (adjustDirection.x != 0) return;
        Toggle_Adjustments(false);
    }
    
    
    // Volume
    public void Set_Volumes(TextMeshProUGUI text)
    {
        if (_adjusting) return;
        
        int textValue = Mathf.RoundToInt(_data.volume * 10f);
        text.text = textValue.ToString();
        
        _selectedTextIndex  = Array.IndexOf(_adjustmentTexts, text);
        OnOptionNavigate += Update_Volume;
    }
    
    public void Update_Volume(Vector2 adjustDirection)
    {
        if (adjustDirection.x == 0) return;
        
        _data.Update_Volume(0.1f * adjustDirection.x);
        
        int textValue = Mathf.RoundToInt(_data.volume * 10f);
        _adjustmentTexts[_selectedTextIndex].text = textValue.ToString();
    }
    
    
    // Resolutions
    public void Set_Resolutions(TextMeshProUGUI text)
    {
        if (_adjusting) return;
        
        _resolutions.Clear();

        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            float aspect = (float)Screen.resolutions[i].width / Screen.resolutions[i].height;
            if (Mathf.Abs(aspect - 16f / 9f) >= 0.01f) continue;
        
            _resolutions.Add(new Vector2Int(Screen.resolutions[i].width, Screen.resolutions[i].height));
        }

        if (_resolutions.Count <= 0)
        {
            foreach (ResolutionData data in _fallBackResolutions)
            {
                _resolutions.Add(new(data.width, data.height));
            }
        }

        _indexNum = _resolutions.Contains(_data.resolution) ? _resolutions.IndexOf(_data.resolution) : 0;
        
        text.text = _data.resolution.x + " x " + _data.resolution.y;

        _selectedTextIndex  = Array.IndexOf(_adjustmentTexts, text);
        OnOptionNavigate += Update_Resolution;
    }

    public void Update_Resolution(Vector2 adjustDirection)
    {
        if (adjustDirection.x == 0) return;

        _indexNum = Mathf.Clamp(_indexNum + (int)adjustDirection.x, 0, _resolutions.Count - 1);
        _data.Set_Resolution(_resolutions[_indexNum]);
        
        string resString =_resolutions[_indexNum].x + " x " + _resolutions[_indexNum].y;
        _adjustmentTexts[_selectedTextIndex].text = resString;
    }


    public void Toggle_ScreenMode(TextMeshProUGUI text)
    {
        _data.Toggle_FullScreen(!_data.isFullScreen);
        Update_ScreenModeText(text);

        if (_adjusting) return;
        Toggle_Adjustments(true);
    }

    private void Update_ScreenModeText(TextMeshProUGUI text)
    {
        if (_data.isFullScreen)
        {
            text.text = "Full Screen";
            return;
        }
        text.text = "window";
    }
    
    
    // Language
    public void Set_Language(TextMeshProUGUI text)
    {
        if (_adjusting) return;

        Localization_Controller localizeController = Localization_Controller.instance;
        string currentLanguage = localizeController.Current_LanguageName();
        
        _indexNum = localizeController.languageNames.IndexOf(currentLanguage);
        text.text = currentLanguage;
        
        _selectedTextIndex  = Array.IndexOf(_adjustmentTexts, text);
        OnOptionNavigate += Update_Language;
    }

    public void Update_Language(Vector2 adjustDirection)
    {
        if (adjustDirection.x == 0) return;

        Localization_Controller localizeController = Localization_Controller.instance;
        int languageCount = localizeController.languageNames.Count;

        _indexNum = (_indexNum + (int)adjustDirection.x + languageCount) % languageCount;
        _adjustmentTexts[_selectedTextIndex].text = localizeController.languageNames[_indexNum];

        _data.Set_Language(localizeController.languageNames[_indexNum].ToString());
    }
}
