using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;

public class Localization_Controller : MonoBehaviour
{
    public static Localization_Controller instance;
    
    
    private List<string> _languageNames = new();
    public List<string> languageNames => _languageNames;
    
    public Action OnLanguageChanged;
    
    
    // UnityEngine
    private void Awake()
    {
        instance = this;
        
        Set_CurrentLanguages();
        
        LocalizationSettings.SelectedLocaleChanged += OnLanguageUpdate;
    }

    private void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLanguageUpdate;
    }

    
    // Control
    private void OnLanguageUpdate(Locale newLocale)
    {
        Debug.Log("Language changed to: " + newLocale);
        OnLanguageChanged?.Invoke();
    }
    
    private void Set_CurrentLanguages()
    {
        List<Locale> locals = LocalizationSettings.AvailableLocales.Locales;
        
        foreach (Locale locale in locals)
        {
            _languageNames.Add(locale.Identifier.CultureInfo.EnglishName);
        }
    }
    
    
    public string Current_LanguageName()
    {
        return LocalizationSettings.SelectedLocale.Identifier.CultureInfo.EnglishName;
    }

    public void Update_Language(string languageName)
    {
        List<Locale> locales = LocalizationSettings.AvailableLocales.Locales;

        foreach (Locale locale in locales)
        {
            if (locale.Identifier.CultureInfo.EnglishName.Equals(languageName, StringComparison.OrdinalIgnoreCase))
            {
                LocalizationSettings.SelectedLocale = locale;
                return;
            }
        }
    }
}
