using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Localization;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.Localization.Components;

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
    [SerializeField] private LocalizeStringEvent[] _adjustmentTextEvents;
    [SerializeField] private GameObject[] _adjustmentArrows;

    [Header("")] 
    [SerializeField] private TextMeshProUGUI _backButtonText;
    [SerializeField] private LocalizedString[] _backButtonStrings;

    [Header("")]
    [SerializeField] private ResolutionData[] _fallBackResolutions;
    [SerializeField] private LocalizedString[] screenTypeStrings;
    
    [Header("")] 
    [SerializeField] private Menu_Controller _applyMenu;
    [SerializeField] private TextMeshProUGUI _applyButtonText;
    [SerializeField] [Range(0, 100)] private int _applyCountTime;
    

    private bool _adjusting;
    private int _selectedTextIndex;
    
    private Action<Vector2> OnAdjustmentNavigate;
    private int _indexNum;

    private OptionsData _previewData;
    private OptionsData _data;

    private Coroutine _applyCoroutine;
    
    private List<Vector2Int> _resolutions = new();
    private Coroutine _resCoroutine;


    // UnityEngine
    private new void Start()
    {
        base.Start();
        
        if (ES3.FileExists("OptionsFile.es3") == false)
        {
            _data = new(1f);
        }
        else
        {
            _data = ES3.Load<OptionsData>("OptionsMenu_Controller/OptionsData", "OptionsFile.es3");
        }
        
        Apply_OptionsData(_data);
        Set_CurrentIndex(eventButtons.Length);

        Refresh_Adjustments();
        
        // subscriptions
        inputManager.OnCursorControl += Navigate_Adjustments;
    }

    private new void OnDestroy()
    {
        base.OnDestroy();
        
        // subscriptions
        inputManager.OnCursorControl -= Navigate_Adjustments;
    }
    
    
    // Options Control
    private void Apply_OptionsData(OptionsData data)
    {
        if (data == null) return;

        RuntimeManager.GetBus("bus:/").setVolume(data.volume);
        Screen.SetResolution(data.resolution.x, data.resolution.y, data.isFullScreen);
        Localization_Controller.instance.Update_Language(data.language);
    }
    public void Apply_OptionsData(bool apply)
    {
        Stop_ApplyCountTime();
        
        OptionsData previousData = new(_previewData);
        _previewData = null;
        
        if (apply) return;
        
        _data = new(previousData);
        
        Apply_OptionsData(_data);
        ES3.Save("OptionsMenu_Controller/OptionsData", _data, "OptionsFile.es3");
    }

    private IEnumerator Apply_OptionsData()
    {
        var handle = _applyButtonText.GetComponent<LocalizeStringEvent>().StringReference.GetLocalizedStringAsync();
        yield return handle;

        string localizedBaseText = handle.Result;
        
        for (int i = _applyCountTime; i >= 0; i--)
        {
            _applyButtonText.text = localizedBaseText + ": " + i.ToString();
            yield return new WaitForSeconds(1);
        }
        
        _applyMenu.OnExitMenu?.Invoke();
        _applyCoroutine = null;
    }
    private void Stop_ApplyCountTime()
    {
        if (_applyCoroutine == null) return;
        
        StopCoroutine(_applyCoroutine);
        _applyCoroutine = null;
    }
    
    public void Save_OptionsData()
    {
        if (_adjusting == false) return;
        if (_previewData == null) return;
        
        if (_previewData == _data)
        {
            _previewData = null;
            return;
        }
        
        OptionsData previousData = new(_data);
        _data = new(_previewData);

        Apply_OptionsData(_data);
        ES3.Save("OptionsMenu_Controller/OptionsData", _data, "OptionsFile.es3");

        if (_data.resolution == previousData.resolution && _data.isFullScreen == previousData.isFullScreen) return;
        
        _previewData = new(previousData);
        Toggle_Menu(false);
        
        _applyMenu.Set_CurrentIndex(0);
        _applyMenu.Toggle_Menu(true);
        
        _applyCoroutine = StartCoroutine(Apply_OptionsData());
    }
    
    
    private void Refresh_Adjustments()
    {
        for (int i = 0; i < _adjustmentTextEvents.Length; i++)
        {
            _adjustmentTextEvents[i].RefreshString();
            _adjustmentArrows[i].SetActive(false);
        }
    }
    
    public void Toggle_Adjustments(bool toggle)
    {
        if (_resCoroutine != null) return;
        
        _adjusting = toggle;
        Update_BackButtonText();

        if (toggle) return;

        OnAdjustmentNavigate = null;
        Refresh_Adjustments();
        
        if (_previewData == null) return;
        Apply_OptionsData(_data);
    }
    public void Toggle_Adjustments(GameObject adjustmentArrow)
    {
        Toggle_Adjustments(!_adjusting);
        adjustmentArrow.SetActive(_adjusting);
    }
    
    private void Navigate_Adjustments(Vector2 direction)
    {
        if (_adjusting == false) return;

        if (direction.y != 0)
        {
            Toggle_Adjustments(false);
            return;
        }
        
        OnAdjustmentNavigate?.Invoke(direction);
    }
    
    
    private void Update_BackButtonText()
    {
        if (_adjusting == false)
        {
            _backButtonText.text = _backButtonStrings[0].GetLocalizedString();
            return;
        }
        _backButtonText.text = _backButtonStrings[1].GetLocalizedString();
    }
    
    
    // Volume
    public void Set_Volumes(TextMeshProUGUI text)
    {
        if (_adjusting) return;
        
        _previewData = new(_data);
        
        int textValue = Mathf.RoundToInt(_data.volume * 10f);
        text.text = textValue.ToString();
        
        _selectedTextIndex  = Array.IndexOf(_adjustmentTexts, text);
        OnAdjustmentNavigate += Update_Volume;
    }
    
    public void Update_Volume(Vector2 adjustDirection)
    {
        if (adjustDirection.x == 0) return;

        _previewData.Update_Volume(0.1f * adjustDirection.x);
        
        int textValue = Mathf.RoundToInt(_previewData.volume * 10f);
        _adjustmentTexts[_selectedTextIndex].text = textValue.ToString();
        
        Update_BackButtonText();
    }
    
    
    // Resolutions
    public void Set_Resolutions(TextMeshProUGUI text)
    {
        if (_adjusting) return;
        
        _previewData = new(_data);
        
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
        OnAdjustmentNavigate += Update_Resolution;
    }

    public void Update_Resolution(Vector2 adjustDirection)
    {
        if (adjustDirection.x == 0) return;

        _indexNum = Mathf.Clamp(_indexNum + (int)adjustDirection.x, 0, _resolutions.Count - 1);
;
        _previewData.Set_Resolution(_resolutions[_indexNum]);
        
        string resString =_resolutions[_indexNum].x + " x " + _resolutions[_indexNum].y;
        _adjustmentTexts[_selectedTextIndex].text = resString;
        
        Update_BackButtonText();
    }


    public void Set_ScreenMode(TextMeshProUGUI text)
    {
        if (_adjusting) return;

        _previewData = new(_data);
        
        _selectedTextIndex  = Array.IndexOf(_adjustmentTexts, text);
        OnAdjustmentNavigate += Update_ScreenMode;

        if (_data.isFullScreen)
        {
            text.text = screenTypeStrings[0].GetLocalizedString();
            return;
        }
        text.text = screenTypeStrings[1].GetLocalizedString();
    }

    private void Update_ScreenMode(Vector2 adjustDirection)
    {
        if (adjustDirection.x == 0) return;

        _previewData.Toggle_FullScreen(!_previewData.isFullScreen);
        
        Update_BackButtonText();
        
        TextMeshProUGUI adjustmentText = _adjustmentTexts[_selectedTextIndex];

        if (_previewData.isFullScreen)
        {
            adjustmentText.text = screenTypeStrings[0].GetLocalizedString();
            return;
        }
        adjustmentText.text = screenTypeStrings[1].GetLocalizedString();
    }
    
    
    // Language
    public void Set_Language(TextMeshProUGUI text)
    {
        if (_adjusting) return;

        _previewData = new(_data);
        
        Localization_Controller localizeController = Localization_Controller.instance;
        string currentLanguage = localizeController.Current_LanguageName();
        
        _indexNum = localizeController.languageNames.IndexOf(currentLanguage);
        text.text = currentLanguage;
        
        _selectedTextIndex  = Array.IndexOf(_adjustmentTexts, text);
        OnAdjustmentNavigate += Update_Language;
    }

    public void Update_Language(Vector2 adjustDirection)
    {
        if (adjustDirection.x == 0) return;

        Localization_Controller localizeController = Localization_Controller.instance;
        int languageCount = localizeController.languageNames.Count;

        _indexNum = (_indexNum + (int)adjustDirection.x + languageCount) % languageCount;
        _adjustmentTexts[_selectedTextIndex].text = localizeController.languageNames[_indexNum];

        _previewData.Set_Language(localizeController.languageNames[_indexNum].ToString());
        
        Update_BackButtonText();
    }
}
