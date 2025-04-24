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

    private OptionsData _data;
    
    
    private List<Resolution> _resolutions = new();


    // UnityEngine
    private new void Awake()
    {
        Set_OptionsData();
    }
    
    private new void Start()
    {
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
    private void Set_OptionsData()
    {
        _data = ES3.Load("OptionsMenu_Controller/OptionsData", new OptionsData(1f));
    }

    public void Save_OptionsData()
    {
        ES3.Save("OptionsMenu_Controller/OptionsData", _data);

        if (_data.resolution == _data.ResolutionVector2(Screen.currentResolution)) return;
        _data.Apply_Resolution();
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
        
        RuntimeManager.GetBus("bus:/").getVolume(out float currentVolume);
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
        
            _resolutions.Add(Screen.resolutions[i]);
        }

        if (_resolutions.Count <= 0)
        {
            foreach (ResolutionData data in _fallBackResolutions)
            {
                Resolution res = new Resolution
                {
                    width = data.width,
                    height = data.height,
                };
                
                _resolutions.Add(res);
            }
        }

        Resolution currentRes = Screen.currentResolution;
        _indexNum = _resolutions.Contains(currentRes) ? _resolutions.IndexOf(Screen.currentResolution) : 0;
        
        text.text = currentRes.width + " x " + currentRes.height;

        _selectedTextIndex  = Array.IndexOf(_adjustmentTexts, text);
        OnOptionNavigate += Update_Resolution;
    }

    public void Update_Resolution(Vector2 adjustDirection)
    {
        if (adjustDirection.x == 0) return;
        _indexNum = (_indexNum + (int)adjustDirection.x + _resolutions.Count) % _resolutions.Count;

        _data.Set_Resolution(_resolutions[_indexNum]);
        
        string resString =_resolutions[_indexNum].width + " x " + _resolutions[_indexNum].height;
        _adjustmentTexts[_selectedTextIndex].text = resString;
    }
    
    
    // Language
}
