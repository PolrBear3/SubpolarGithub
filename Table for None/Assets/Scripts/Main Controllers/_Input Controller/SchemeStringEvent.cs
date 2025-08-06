using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SchemeStringEvent : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _updateText;
    
    
    // UnityEngine
    private void Start()
    {
        Input_Controller input = Input_Controller.instance;
        input.Update_EmojiAsset(_updateText);
        
        input.OnSchemeUpdate += () => input.Update_EmojiAsset(_updateText);
    }

    private void OnDestroy()
    {
        Input_Controller input = Input_Controller.instance;
        input.OnSchemeUpdate -= () => input.Update_EmojiAsset(_updateText);
    }
}
