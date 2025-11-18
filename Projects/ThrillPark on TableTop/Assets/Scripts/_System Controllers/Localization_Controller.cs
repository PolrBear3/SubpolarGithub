using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEditor;

public class Localization_Controller : MonoBehaviour
{
    public static Localization_Controller instance;
    
    [Space(20)]
    [SerializeField] private string[] _tableReferences;
    
    private List<string> _languageNames = new();
    public List<string> languageNames => _languageNames;
    
    public Action OnLocalizationLoad;
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
        
        await Load_LocalizationTables();
        OnLocalizationLoad?.Invoke();
        
        Set_CurrentLanguages();

        LocalizationSettings.SelectedLocaleChanged += OnLanguageUpdate;
    }

    private void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLanguageUpdate;
    }
    
    
    // Control
    private async Task Load_LocalizationTables()
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
        
        await Load_LocalizationTables();
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


#if UNITY_EDITOR
[CustomEditor(typeof(Localization_Controller))]
public class Localization_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        Localization_Controller controller = (Localization_Controller)target;

        base.OnInspectorGUI();

        GUILayout.Space(20);

        if (GUILayout.Button("Update Language"))
        {
            List<string> languages = controller.languageNames;
            string currentLanguage = controller.Current_LanguageName();

            for (int i = 0; i < languages.Count; i++)
            {
                if (currentLanguage != languages[i]) continue;

                i = (i + 1 + languages.Count) % languages.Count;
                controller.Update_Language(languages[i]);

                break;
            }
        }
    }
}
#endif