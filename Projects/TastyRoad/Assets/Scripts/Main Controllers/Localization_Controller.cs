using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;

public class Localization_Controller : MonoBehaviour
{
    public static Localization_Controller instance;
    
    public Action OnLanguageChanged;
    
    
    // UnityEngine
    private void Awake()
    {
        instance = this;
        
        LocalizationSettings.SelectedLocaleChanged += OnLanguageUpdate;
    }

    private void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLanguageUpdate;
    }

    
    // Control
    private void OnLanguageUpdate(Locale newLocale)
    {
        Debug.unityLogger.Log("Language changed to: " + newLocale);
        OnLanguageChanged?.Invoke();
    }
}
