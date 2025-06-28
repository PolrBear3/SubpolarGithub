using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfoBox_Controller : MonoBehaviour
{
    public static InfoBox_Controller instance;
    
    
    [Space(20)]
    [SerializeField] private Image _infoBoxPanel;
    public Image infoBoxPanel => _infoBoxPanel;
    
    [SerializeField] private TextMeshProUGUI _infoText;


    private Vector2 _defaultPosition;
    
    
    // MonoBehaviour
    private void Awake()
    {
        instance = this;
        
        _defaultPosition = _infoBoxPanel.rectTransform.anchoredPosition;
    }
    
    
    // Main
    public void Update_InfoText(string updateText, Vector2 updatePosition)
    {
        _infoBoxPanel.gameObject.SetActive(true);
        
        _infoBoxPanel.rectTransform.anchoredPosition = updatePosition;
        _infoText.text = updateText;
    }
    public void Update_InfoText(string updateText)
    {
        Update_InfoText(updateText, _defaultPosition);
    }
}
