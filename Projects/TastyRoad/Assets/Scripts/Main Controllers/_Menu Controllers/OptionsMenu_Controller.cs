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
    [Space(20)]
    [SerializeField] private TextMeshProUGUI[] _adjustmentTexts;
    [SerializeField] private LocalizeStringEvent[] _adjustmentTextEvents;
    [SerializeField] private GameObject[] _adjustmentArrows;

    [Space(20)] 
    [SerializeField] private GameObject _saveButton;
    
    [Space(20)] 
    [SerializeField] private Menu_Controller _volumeMenu;
    [SerializeField] private TextMeshProUGUI[] _volumeTexts;
    [SerializeField] private GameObject[] _volumeArrows;

    private List<string> _volumeStrings = new();
    
    [Space(20)]
    [SerializeField] private ResolutionData[] _fallBackResolutions;
    [SerializeField] private LocalizedString[] screenTypeStrings;
    
    [Space(20)]
    [SerializeField] private Menu_Controller _applyMenu;
    [SerializeField] private TextMeshProUGUI _applyButtonText;
    [SerializeField] [Range(0, 100)] private int _applyCountTime;
    

    private bool _adjusting;
    private int _selectedTextIndex;
    
    private Action<Vector2> OnAdjustmentNavigate;
    private int _adjustIndexNum;

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
        _saveButton.SetActive(_adjusting);
        
        // save volume texts
        foreach (TextMeshProUGUI text in _volumeTexts)
        {
            _volumeStrings.Add(text.text);
        }
        
        // subscriptions
        inputManager.OnCursorControl += Navigate_Adjustments;
        
        Input_Manager volumeMenuInput = _volumeMenu.inputManager;
        
        volumeMenuInput.OnCursorControl += UpdateBGM_Volume;
        volumeMenuInput.OnCursorControl += UpdateSFX_Volume;
    }

    private new void OnDestroy()
    {
        base.OnDestroy();
        
        // subscriptions
        inputManager.OnCursorControl -= Navigate_Adjustments;

        Input_Manager volumeMenuInput = _volumeMenu.inputManager;
        
        volumeMenuInput.OnCursorControl -= UpdateBGM_Volume;
        volumeMenuInput.OnCursorControl -= UpdateSFX_Volume;
    }
    
    
    // Menu_Controller
    public new void Navigate_ButtonIndex(int index)
    {
        if (_adjusting) return;
        base.Navigate_ButtonIndex(index);
    }
    
    public void Select_Action(int index)
    {
        if (_adjusting == false || currentIndex == index)
        {
            base.Select_Action();
            return;
        }
        
        /*
        Toggle_Adjustments(false);
            
        base.Navigate_ButtonIndex(index);
        base.Select_Action();
        */
    }
    
    
    // Options Control
    private void Apply_OptionsData(OptionsData data)
    {
        if (data == null) return;

        RuntimeManager.GetBus("bus:/BGM").setVolume(data.bgmVolume);
        RuntimeManager.GetBus("bus:/SFX").setVolume(data.sfxVolume);
        
        Screen.SetResolution(data.resolution.x, data.resolution.y, data.isFullScreen);
        Localization_Controller.instance.Update_Language(data.language);
    }
    public void Apply_OptionsData(bool apply)
    {
        Stop_ApplyCountTime();
        
        OptionsData previousData = new(_previewData);
        _previewData = null;

        if (apply)
        {
            Save_CurrentData();
            return;
        }
        
        _data = new(previousData);
        
        Apply_OptionsData(_data);
        Save_CurrentData();
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


    private void Save_CurrentData()
    {
        if (_data == null) return;
        ES3.Save("OptionsMenu_Controller/OptionsData", _data, "OptionsFile.es3");
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
        _saveButton.SetActive(_adjusting);

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
    public void Navigate_Adjustments(float directionX)
    {
        Navigate_Adjustments(new Vector2(directionX, 0));
    }
    
    
    // Volume
    public void Refresh_VolumeAdjustments()
    {
        _adjusting = false;
        
        for (int i = 0; i < _volumeTexts.Length; i++)
        {
            _volumeTexts[i].text = _volumeStrings[i];
            _volumeArrows[i].SetActive(false);
        }
    }
    
    public void Set_Volumes(TextMeshProUGUI text)
    {
        _adjusting = !_adjusting;

        if (_adjusting == false)
        {
            Refresh_VolumeAdjustments();
            return;
        }

        float currentVolume = text.text == "BGM" ? _data.bgmVolume : _data.sfxVolume;
            
        int textValue = Mathf.RoundToInt(currentVolume * 10f);
        text.text = textValue.ToString();
        
        _selectedTextIndex  = Array.IndexOf(_volumeTexts, text);
        _volumeArrows[_selectedTextIndex].SetActive(true);
    }
    
    
    private void UpdateBGM_Volume(Vector2 adjustDirection)
    {
        if (_adjusting == false) return;
        
        if (adjustDirection.x == 0)
        {
            Refresh_VolumeAdjustments();
            return;
        }
        
        if (_volumeStrings[_selectedTextIndex] != "BGM") return;

        _data.UpdateBGM_Volume(0.1f * adjustDirection.x);
        Save_CurrentData();
        
        int textValue = Mathf.RoundToInt(_data.bgmVolume * 10f);
        _volumeTexts[_selectedTextIndex].text = textValue.ToString();
    }
    public void UpdateBGM_Volume(int directionX)
    {
        UpdateBGM_Volume(new Vector2(directionX, 0));
    }
    
    private void UpdateSFX_Volume(Vector2 adjustDirection)
    {
        if (_adjusting == false) return;

        if (adjustDirection.x == 0)
        {
            Refresh_VolumeAdjustments();
            return;
        }
        
        if(_volumeStrings[_selectedTextIndex] != "SFX") return;
        
        _data.UpdateSFX_Volume(0.1f * adjustDirection.x);
        Save_CurrentData();
        
        int textValue = Mathf.RoundToInt(_data.sfxVolume * 10f);
        _volumeTexts[_selectedTextIndex].text = textValue.ToString();
    }
    public void UpdateSFX_Volume(int directionX)
    {
        UpdateSFX_Volume(new Vector2(directionX, 0));
    }
    
    
    // Resolutions
    public void Set_Resolutions(TextMeshProUGUI text)
    {
        if (_adjusting) return;
        
        _previewData = new(_data);
        _resolutions.Clear();

        ResolutionData[] fallback = _fallBackResolutions;
        
        foreach (ResolutionData data in fallback)
        {
            Vector2Int resolution = new(data.width, data.height);
            if (!_resolutions.Contains(resolution))
            {
                _resolutions.Add(resolution);
            }
        }

        Vector2Int currentRes = new Vector2Int(Display.main.systemWidth, Display.main.systemHeight);
        if (!_resolutions.Contains(currentRes))
        {
            _resolutions.Add(currentRes);
        }

        _adjustIndexNum = _resolutions.Contains(_data.resolution) ? _resolutions.IndexOf(_data.resolution) : 0;
        
        text.text = _data.resolution.x + " x " + _data.resolution.y;

        _selectedTextIndex  = Array.IndexOf(_adjustmentTexts, text);
        OnAdjustmentNavigate += Update_Resolution;
    }

    public void Update_Resolution(Vector2 adjustDirection)
    {
        if (adjustDirection.x == 0) return;

        _adjustIndexNum = Mathf.Clamp(_adjustIndexNum + (int)adjustDirection.x, 0, _resolutions.Count - 1);
;
        _previewData.Set_Resolution(_resolutions[_adjustIndexNum]);
        
        string resString =_resolutions[_adjustIndexNum].x + " x " + _resolutions[_adjustIndexNum].y;
        _adjustmentTexts[_selectedTextIndex].text = resString;
        
        _saveButton.SetActive(_adjusting);
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
        
        _saveButton.SetActive(_adjusting);
        
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
        
        _adjustIndexNum = localizeController.languageNames.IndexOf(currentLanguage);
        text.text = currentLanguage;
        
        _selectedTextIndex  = Array.IndexOf(_adjustmentTexts, text);
        OnAdjustmentNavigate += Update_Language;
    }

    public void Update_Language(Vector2 adjustDirection)
    {
        if (adjustDirection.x == 0) return;

        Localization_Controller localizeController = Localization_Controller.instance;
        int languageCount = localizeController.languageNames.Count;

        _adjustIndexNum = (_adjustIndexNum + (int)adjustDirection.x + languageCount) % languageCount;
        _adjustmentTexts[_selectedTextIndex].text = localizeController.languageNames[_adjustIndexNum];

        _previewData.Set_Language(localizeController.languageNames[_adjustIndexNum].ToString());
        
        _saveButton.SetActive(_adjusting);
    }
}
