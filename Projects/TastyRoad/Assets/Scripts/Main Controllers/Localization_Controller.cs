using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.Serialization;

public class Localization_Controller : MonoBehaviour
{
    public static Localization_Controller instance;
    
    [Header("")]
    [SerializeField] private string[] _tableReferences;
    
    private List<string> _languageNames = new();
    public List<string> languageNames => _languageNames;
    
    public Action OnLocalizationReady;
    public Action OnLanguageChanged;
    
    
    // UnityEngine
    private async void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        
        await PreloadAllLocalization();
        Set_CurrentLanguages();
        OnLocalizationReady?.Invoke();
        
        LocalizationSettings.SelectedLocaleChanged += OnLanguageUpdate;
    }

    private void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLanguageUpdate;
    }
    
    
    // Control
    private async Task PreloadAllLocalization()
    {
        string[] TableNames = _tableReferences; 

        await LocalizationSettings.InitializationOperation.Task;

        foreach (var tableName in TableNames)
        {
            var handle = LocalizationSettings.StringDatabase.GetTableAsync(tableName);
            await handle.Task;

            if (handle.Status != UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                Debug.LogWarning($"Failed to preload String Table: {tableName}");
            }
        }
    }
    
    private async void OnLanguageUpdate(Locale newLocale)
    {
        Debug.Log("Language changed to: " + newLocale);
        
        await PreloadAllLocalization();
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
