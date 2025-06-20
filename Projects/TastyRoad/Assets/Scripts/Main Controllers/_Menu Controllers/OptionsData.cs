using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using FMODUnity;
using FMOD.Studio;

[System.Serializable]
public class OptionsData
{
    [ES3Serializable] private float _bgmVolume;
    public float bgmVolume => _bgmVolume;
    
    [ES3Serializable] private float _sfxVolume;
    public float sfxVolume => _sfxVolume;
    
    [ES3Serializable] private Vector2Int _resolution;
    public Vector2Int resolution => _resolution;
    
    [ES3Serializable] private bool _isFullScreen;
    public bool isFullScreen => _isFullScreen;
    
    [ES3Serializable] private string _language;
    public string language => _language;


    // New
    public OptionsData(OptionsData data)
    {
        _bgmVolume = data.bgmVolume;
        _sfxVolume = data.sfxVolume;
        
        _resolution = data.resolution;
        _isFullScreen = data.isFullScreen;
        
        _language = data.language;
    }
    
    public OptionsData(float setVolume, Vector2Int setResolution)
    {
        _bgmVolume = setVolume;
        _sfxVolume = setVolume;

        _resolution = setResolution;
        _isFullScreen = false;
        
        _language = LocalizationSettings.SelectedLocale.Identifier.CultureInfo.NativeName;
    }


    // Data
    private void Update_BusVolume(Bus bus, float updateValue)
    {
        bus.getVolume(out float currentVolume);
       
        float setValue = currentVolume + updateValue;
        setValue = Mathf.Round(setValue * 10f) / 10f;
        setValue = Mathf.Clamp(setValue, 0f, 1f);
        
        bus.setVolume(setValue);
    }
    
    public void UpdateBGM_Volume(float updateValue)
    {
        Bus bus = RuntimeManager.GetBus("bus:/BGM");
        Update_BusVolume(bus, updateValue);
        
        bus.getVolume(out float updatedVolume);
        _bgmVolume = updatedVolume;
    }
    public void UpdateSFX_Volume(float updateValue)
    {
        Bus bus = RuntimeManager.GetBus("bus:/SFX");
        Update_BusVolume(bus, updateValue);
        
        bus.getVolume(out float updatedVolume);
        _sfxVolume = updatedVolume;
    }


    public Vector2Int Resolution_Vector2(Resolution resolution)
    {
        return new Vector2Int(resolution.width, resolution.height);
    }

    public void Set_Resolution(Vector2Int resolution)
    {
        _resolution = new Vector2Int(resolution.x, resolution.y);
    }


    public void Toggle_FullScreen(bool isFullScreen)
    {
        _isFullScreen = isFullScreen;
    }

    public void Set_Language(string language)
    {
        _language = language;
    }
}