using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Input_Controller : MonoBehaviour
{
    public static Input_Controller instance;


    [Header("")]
    [SerializeField] private PlayerInput _playerInput;

    [SerializeField] private ControlScheme_ScrObj[] _schemes;

    [Header("")]
    [SerializeField][Range(0, 100)] private float _holdTime;


    private float _currentHoldTime;
    public float currentHoldTime => _currentHoldTime;

    private ControlScheme_ScrObj _currentScheme;
    public ControlScheme_ScrObj currentScheme => _currentScheme;

    private List<string> _actionMaps = new();


    private Vector2 _inputDirection;
    public Vector2 inputDirection => _inputDirection;

    private bool _isHolding;
    public bool isHolding => _isHolding;

    private bool _action1Pressed;
    public bool action1Pressed => _action1Pressed;

    private bool _action2Pressed;
    public bool action2Pressed => _action2Pressed;


    public Action OnSchemeUpdate;


    public Action<Vector2> OnMovement;
    public Action<InputActionReference> OnAnyInput;

    public Action OnInteractStart;
    public Action OnInteract;
    public Action OnHoldInteract;

    public Action OnAction1;
    public Action OnAction2;


    public Action<Vector2> OnCursorControl;

    public Action OnSelectStart;
    public Action OnSelect;
    public Action OnHoldSelect;

    public Action OnOption1;
    public Action OnOption2;

    public Action OnExit;


    // MonoBehaviour
    private void Awake()
    {
        Set_Instance();
        Set_ActionMaps();
        Set_CurrentScheme();

        PlayerInput[] allInputs = FindObjectsOfType<PlayerInput>();

        // subscription
        _playerInput.onControlsChanged += Handle_SchemeUpdate;
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


    private void Set_ActionMaps()
    {
        for (int i = 0; i < _playerInput.actions.actionMaps.Count; i++)
        {
            _actionMaps.Add(_playerInput.actions.actionMaps[i].name);
        }
    }

    public void Update_ActionMap(int indexNum)
    {
        if (indexNum < 0 || indexNum >= _actionMaps.Count) return;

        indexNum = Mathf.Clamp(indexNum, 0, _actionMaps.Count - 1);
        string mapName = _actionMaps[indexNum];

        _playerInput.SwitchCurrentActionMap(mapName);
    }


    public InputActionReference ActionReference(string actionName)
    {
        ActionKey_Data[] datas = _currentScheme.actionKeyDatas;

        for (int i = 0; i < datas.Length; i++)
        {
            if (datas[i].actionRef.action.name != actionName) continue;
            return datas[i].actionRef;
        }
        return null;
    }


    // Scheme Control
    private void Handle_SchemeUpdate(PlayerInput playerInput)
    {
        Set_CurrentScheme();

        OnSchemeUpdate?.Invoke();
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

    public GameObject CurrentScheme_ActionKey(InputActionReference reference)
    {
        ActionKey_Data[] datas = _currentScheme.actionKeyDatas;

        for (int i = 0; i < datas.Length; i++)
        {
            if (reference != datas[i].actionRef) continue;
            return datas[i].actionKey;
        }
        return null;
    }


    private void Set_CurrentScheme()
    {
        string schemeName = _playerInput.currentControlScheme;
        Debug.Log(schemeName);

        _currentScheme = ControlScheme("PC");
    }


    // InGame
    private void Inovke_AnyInput(InputAction.CallbackContext context)
    {
        InputActionReference actionRef = ActionReference(context.action.name);
        if (actionRef == null) return;

        OnAnyInput?.Invoke(actionRef);
    }


    public void Movement(InputAction.CallbackContext context)
    {
        Vector2 directionInput = context.ReadValue<Vector2>();

        OnMovement?.Invoke(directionInput);
        _inputDirection = directionInput;
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _isHolding = true;
            _currentHoldTime = Time.time;

            OnInteractStart?.Invoke();
            return;
        }

        if (context.canceled == false) return;

        _isHolding = false;

        if (Time.time - _currentHoldTime >= _holdTime)
        {
            OnHoldInteract?.Invoke();
            Inovke_AnyInput(context);
            return;
        }

        OnInteract?.Invoke();
        Inovke_AnyInput(context);
    }

    public void Action1(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _action1Pressed = true;
        }
        else if (context.canceled)
        {
            _action1Pressed = false;
        }

        if (context.performed == false) return;

        OnAction1?.Invoke();
        Inovke_AnyInput(context);
    }

    public void Action2(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _action2Pressed = true;
        }
        else if (context.canceled)
        {
            _action2Pressed = false;
        }

        if (context.performed == false) return;

        OnAction2?.Invoke();
        Inovke_AnyInput(context);
    }


    // UI Control
    public void CursorControl(InputAction.CallbackContext context)
    {
        Vector2 directionInput = context.ReadValue<Vector2>();

        OnCursorControl?.Invoke(directionInput);
        _inputDirection = directionInput;
    }

    public void Select(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _isHolding = true;
            _currentHoldTime = Time.time;

            OnSelectStart?.Invoke();
            return;
        }

        if (context.canceled == false) return;

        _isHolding = false;

        if (Time.time - _currentHoldTime >= _holdTime)
        {
            OnHoldSelect?.Invoke();
            return;
        }

        OnSelect?.Invoke();
    }

    public void Option1(InputAction.CallbackContext context)
    {
        if (context.performed == false) return;
        OnOption1?.Invoke();
    }

    public void Option2(InputAction.CallbackContext context)
    {
        if (context.performed == false) return;
        OnOption2?.Invoke();
    }

    public void Exit(InputAction.CallbackContext context)
    {
        if (context.performed == false) return;
        OnExit?.Invoke();
    }
}