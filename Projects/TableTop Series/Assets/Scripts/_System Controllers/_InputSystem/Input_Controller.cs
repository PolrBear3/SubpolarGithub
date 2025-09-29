using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;
using TMPro;

public class Input_Controller : MonoBehaviour
{
    public static Input_Controller instance;

    
    [SerializeField] private PlayerInput _playerInput;
    public PlayerInput playerInput => _playerInput;

    [Space(20)]
    [SerializeField] private ControlScheme_ScrObj[] _schemes;

    
    private ControlScheme_ScrObj _currentScheme;
    public ControlScheme_ScrObj currentScheme => _currentScheme;

    private string _currentSchemeName;
    

    public Action OnSchemeUpdate;

    public Action OnSelect;
    public Action OnMultiSelect;
    public Action OnExit;

    
    // MonoBehaviour
    private void Awake()
    {
        Set_Instance();
        Handle_SchemeUpdate(_playerInput);
  
        // subscription
        _playerInput.onControlsChanged += Handle_SchemeUpdate;
    }

    private void Update()
    {
        CurrentScheme_Update();
    }
    
    private void OnDestroy()
    {
        // subscription
        _playerInput.onControlsChanged -= Handle_SchemeUpdate;
    }
    
    
    // Data Control
    private void Set_Instance()
    {
        if (instance != null) return;

        instance = this;
    }


    // Scheme Control
    private void CurrentScheme_Update()
    {
        if (_currentSchemeName == _playerInput.currentControlScheme) return;
        _currentSchemeName = _playerInput.currentControlScheme;
        
        Update_CurrentScheme(_currentSchemeName);
    }
    
    private void Handle_SchemeUpdate(PlayerInput playerInput)
    {
        Update_CurrentScheme(_playerInput.currentControlScheme);
    }
    
    
    public void Update_CurrentScheme(string schemeName)
    {
        _currentScheme = ControlScheme(schemeName);
        OnSchemeUpdate?.Invoke();
        
        Debug.Log("_currentScheme: " + _currentScheme.name + "/ _playerInput.currentControlScheme: " + _playerInput.currentControlScheme);
    }
    
    public void Update_EmojiAsset(TextMeshProUGUI text)
    {
        text.spriteAsset = _currentScheme.emojiAsset;
    }
    
    
    private ControlScheme_ScrObj ControlScheme(string name)
    {
        for (int i = 0; i < _schemes.Length; i++)
        {
            if (_schemes[i].schemeName != name) continue;
            return _schemes[i];
        }
        return null;
    }


    // InGame
    public void Select(InputAction.CallbackContext context)
    {
        if (context.performed == false) return;
        OnSelect?.Invoke();
    }
    
    public void MultiSelect(InputAction.CallbackContext context)
    {
        if (context.performed == false) return;
        OnMultiSelect?.Invoke();
    }
    
    public void Exit(InputAction.CallbackContext context)
    {
        if (context.performed == false) return;
        OnExit?.Invoke();
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(Input_Controller))]
public class Input_Controller_Inspector : Editor
{
    public override void OnInspectorGUI()
    {
        Input_Controller controller = (Input_Controller)target;

        base.OnInspectorGUI();
        serializedObject.Update();

        GUILayout.Space(60);

        if (GUILayout.Button("Toggle Scheme"))
        {
            if (controller.currentScheme.schemeName == "PC")
            {
                controller.Update_CurrentScheme("GamePad");
                return;
            }
            controller.Update_CurrentScheme("PC");
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif