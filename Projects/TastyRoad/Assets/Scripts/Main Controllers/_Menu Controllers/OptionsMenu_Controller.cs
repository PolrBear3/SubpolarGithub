using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu_Controller : Menu_Controller
{
    [Header("")]
    [SerializeField] private Image _subPanel;
    public Image subPanel => _subPanel;

    [SerializeField] private float _panelSeperationValue;
    
    private List<Resolution> _resolutions = new();
    
    
    // UnityEngine
    private new void Start()
    {
        Set_CurrentIndex(eventButtons.Length);
        base.Start();

        Toggle_Menu(false);
        
        Update_Resolutions();
    }
    
    
    // Panel
    public void Toggle_MainPanel(bool toggle)
    {
        if (toggle)
        {
            Toggle_Menu(true);
            return;
        }

        if (_subPanel.gameObject.activeSelf)
        {
            Toggle_SubPanel(false);
            return;
        }
        
        Toggle_Menu(false);
    }
    
    public void Toggle_SubPanel(bool toggle)
    {
        _subPanel.gameObject.SetActive(toggle);
        // back / save button toggle //

        float height = menuPanel.rectTransform.localPosition.y;

        if (toggle)
        {
            menuPanel.rectTransform.localPosition = new Vector2(-_panelSeperationValue, height);
            _subPanel.rectTransform.localPosition = new Vector2(_panelSeperationValue, height);

            return;
        }
        
        menuPanel.rectTransform.localPosition = new Vector2(0f, height);
        _subPanel.rectTransform.localPosition = new Vector2(0f, height);
    }
    
    
    // Volume
    
    
    // Resolutions
    private void Update_Resolutions()
    {
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            float aspect = (float)Screen.resolutions[i].width / Screen.resolutions[i].height;
            if (Mathf.Abs(aspect - 16f / 9f) >= 0.01f) continue;
            
            _resolutions.Add((Resolution)Screen.resolutions[i]);
        }
    }
}
