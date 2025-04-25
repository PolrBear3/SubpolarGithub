using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using FMODUnity;
using FMOD.Studio;

[System.Serializable]
public class OptionsData
{
    [ES3Serializable] private float _volume;
    public float volume => _volume;
    
    [ES3Serializable] private Vector2Int _resolution;
    public Vector2Int resolution => _resolution;
    
    [ES3Serializable] private bool _isFullScreen;
    public bool isFullScreen => _isFullScreen;
    
    [ES3Serializable] private string _language;
    public string language => _language;


    // New
    public OptionsData(OptionsData data)
    {
        _volume = data.volume;
        _resolution = data.resolution;
        _isFullScreen = data.isFullScreen;
        _language = data.language;
    }
    
    public OptionsData(float volume)
    {
        _volume = volume;

        _resolution = Resolution_Vector2(Screen.currentResolution);
        _isFullScreen = false;
        
        _language = LocalizationSettings.SelectedLocale.Identifier.CultureInfo.NativeName;
    }


    // Data
    public void Update_Volume(float updateValue)
    {
        Bus bus = RuntimeManager.GetBus("bus:/");
        bus.getVolume(out float currentVolume);

        float setValue = currentVolume + updateValue;
        
        setValue = Mathf.Round(setValue * 10f) / 10f;
        setValue = Mathf.Clamp(setValue, 0f, 1f);
        
        bus.setVolume(setValue);
        
        bus.getVolume(out float updatedVolume);
        _volume = updatedVolume;
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