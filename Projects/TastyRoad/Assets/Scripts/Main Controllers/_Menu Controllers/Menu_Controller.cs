using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Menu_EventButton
{
    [SerializeField] private Image _button;
    public Image button => _button;

    [SerializeField] private Image _hoverIndication;
    public Image hoverIndication => _hoverIndication;
    
    [Space(20)]
    [SerializeField] private UnityEvent _actionEvent;
    public UnityEvent actionEvent => _actionEvent;
    
    [Space(10)]
    [SerializeField] private UnityEvent[] _navigateEvents;

    
    private Vector2 _defaultPosition;
    public Vector2 defaultPosition => _defaultPosition;


    public void Set_DefaultPosition()
    {
        _defaultPosition = button.rectTransform.anchoredPosition;
    }

    public void Invoke_NavigateEvents()
    {
        foreach (UnityEvent navigateEvent in _navigateEvents)
        {
            navigateEvent.Invoke();
        }
    }
}

public class Menu_Controller : MonoBehaviour
{
    [Header("")]
    [SerializeField] private Image _menuPanel;
    public Image menuPanel => _menuPanel;
    
    [Header("")] 
    [SerializeField] private Menu_EventButton[] _eventButtons;
    public Menu_EventButton[] eventButtons => _eventButtons;

    [Header("")] 
    public UnityEvent OnExitMenu;
    private Action OnExit;

    private bool _toggled;
    
    private int _currentIndex;
    public int currentIndex => _currentIndex;

    public Action OnNavigate;
    public Action OnAction;
    
    
    [Space(80)]
    [SerializeField] private Input_Manager _inputManager;
    public Input_Manager inputManager => _inputManager;


    // MonoBehaviour
    public void Awake()
    {
        
    }

    public void Start()
    {
        // subscriptions
        _inputManager.OnCursorControl += Navigate_ButtonIndex;
        _inputManager.OnSelect += Select_Action;
        
        OnExit = () => OnExitMenu?.Invoke();
        _inputManager.OnExit += OnExit;
        
        foreach (Menu_EventButton button in _eventButtons)
        {
            button.Set_DefaultPosition();
        }
    }

    public void OnDestroy()
    {
        // subscriptions
        _inputManager.OnCursorControl -= Navigate_ButtonIndex;
        _inputManager.OnSelect -= Select_Action;
        
        _inputManager.OnExit -= OnExit;
        OnExit = null;
    }
    

    // Data Control
    public void Set_CurrentIndex(int indexNum)
    {
        _currentIndex = Mathf.Clamp(indexNum, 0, _eventButtons.Length - 1);
    }


    // Control
    public bool Menu_Toggled()
    {
        return _menuPanel.gameObject.activeSelf;
    }
    
    public void Toggle_Menu(bool toggle)
    {
        Input_Controller input = Input_Controller.instance;

        if (_menuPanel != null) _menuPanel.gameObject.SetActive(toggle);

        if (toggle && _toggled == false)
        {
            _toggled = true;
            _inputManager.Toggle_Input(true);

            NavigateUpdate_EventButtons();
            return;
        }

        if (toggle || _toggled == false) return;

        _toggled = false;
        _inputManager.Toggle_Input(false);
    }


    public void Navigate_ButtonIndex(int index)
    {
        if (TransitionCanvas_Controller.instance.coroutine != null) return;
        
        _currentIndex = Mathf.Clamp(index, 0, _eventButtons.Length - 1);
        NavigateUpdate_EventButtons();
    }
    public void Navigate_ButtonIndex(Vector2 direction)
    {
        if (direction.y == 0) return;
        if (TransitionCanvas_Controller.instance.coroutine != null) return;
        
        int actionCount = _eventButtons.Length;
        if (actionCount <= 0) return;

        _currentIndex = (_currentIndex + (int)direction.y + actionCount) % actionCount;
        NavigateUpdate_EventButtons();
    }

    public void NavigateUpdate_EventButtons()
    {
        for (int i = 0; i < _eventButtons.Length; i++)
        {
            RectTransform buttonTransform = _eventButtons[i].button.rectTransform;
            
            if (i != _currentIndex)
            {
                buttonTransform.localPosition = _eventButtons[i].defaultPosition;
                _eventButtons[i].hoverIndication.gameObject.SetActive(false);
                continue;
            }

            buttonTransform.localPosition = Vector2.zero;
            _eventButtons[i].hoverIndication.gameObject.SetActive(true);
        }
        
        OnNavigate?.Invoke();
        eventButtons[_currentIndex].Invoke_NavigateEvents();
        
        // sound
        Audio_Controller.instance.Play_OneShot(gameObject, 1);
    }

    
    public void Select_Action()
    {
        if (TransitionCanvas_Controller.instance.coroutine != null) return;
        if (_eventButtons.Length <= 0) return;

        _eventButtons[_currentIndex].actionEvent?.Invoke();
        OnAction?.Invoke();
    }

    public void Exit()
    {
        OnExit?.Invoke();
    }
}
