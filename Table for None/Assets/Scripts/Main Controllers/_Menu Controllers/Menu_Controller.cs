using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TMPro;

[System.Serializable]
public class Menu_EventButton
{
    [SerializeField] private Image _selectIcon;
    public Image selectIcon => _selectIcon;
    
    [SerializeField] private Image _button;
    public Image button => _button;

    [SerializeField] private Image _hoverIndication;
    public Image hoverIndication => _hoverIndication;
    
    [SerializeField] private TextMeshProUGUI _buttonText;
    public TextMeshProUGUI buttonText => _buttonText;
    
    
    [Space(20)]
    [SerializeField] private UnityEvent _actionEvent;
    public UnityEvent actionEvent => _actionEvent;
    
    [Space(10)]
    [SerializeField] private UnityEvent[] _navigateEvents;

    
    private Vector2 _defaultPosition;
    public Vector2 defaultPosition => _defaultPosition;


    // Main
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

    public void Set_SelectIcon(Sprite iconSprite)
    {
        _selectIcon.gameObject.SetActive(iconSprite != null);
        
        if (iconSprite == null) return;
        _selectIcon.sprite = iconSprite;
    }
}

public class Menu_Controller : MonoBehaviour
{
    [Space(20)]
    [SerializeField] private UI_EffectController _uiEffectController;

    [SerializeField] private Image _menuPanel;
    public Image menuPanel => _menuPanel;
    
    [Space(20)]
    [SerializeField] private Menu_EventButton[] _eventButtons;
    public Menu_EventButton[] eventButtons => _eventButtons;
    
    [SerializeField] [Range(0, 100)] private float _maxTextSize;

    [Space(20)]
    public UnityEvent OnExitMenu;

    [Space(20)]
    [SerializeField] private Input_Manager _inputManager;
    public Input_Manager inputManager => _inputManager;
    
    
    private bool _toggled;

    private int _currentIndex;
    public int currentIndex => _currentIndex;

    
    private Action OnExit;
    
    public Action OnNavigate;
    public Action<int> OnNavigateX;
    public Action<int> OnWrapAroundY;
    
    public Action OnAction;
    

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
    

    // Gets
    public List<Menu_EventButton> EventButtons_InOrder()
    {
        List<Menu_EventButton> eventButtons = new();

        for (int i = _eventButtons.Length - 1; i >= 0; i--)
        {
            eventButtons.Add(_eventButtons[i]);
        }
        
        return eventButtons;
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

            Update_TextSize();
            
            _currentIndex = _eventButtons.Length - 1;
            NavigateUpdate_EventButtons();

            if (_uiEffectController == null) return;
            _uiEffectController.Update_Scale(_menuPanel.rectTransform);
            
            return;
        }

        if (toggle || _toggled == false) return;

        _toggled = false;
        _inputManager.Toggle_Input(false);
    }

    
    public void Navigate_ButtonIndex(Vector2 direction)
    {
        if (direction.y == 0)
        {
            OnNavigateX?.Invoke((int)direction.x);
            OnNavigate?.Invoke();
            
            // sound
            Audio_Controller.instance.Play_OneShot(gameObject, 1);
            
            return;
        }
            
        if (TransitionCanvas_Controller.instance.coroutine != null) return;
        
        int actionCount = _eventButtons.Length;
        if (actionCount <= 0) return;

        int previousIndex = _currentIndex;
        _currentIndex = (_currentIndex + (int)direction.y + actionCount) % actionCount;
        
        NavigateUpdate_EventButtons();
        
        bool toLast = previousIndex == 0 && _currentIndex == actionCount - 1;
        bool toFirst = previousIndex == actionCount - 1 && _currentIndex == 0;

        if (toLast == false && toFirst == false) return;
        OnWrapAroundY?.Invoke(toLast ? 1 : -1);
    }
    public void Navigate_ButtonIndex(int index)
    {
        if (TransitionCanvas_Controller.instance.coroutine != null) return;
        
        _currentIndex = Mathf.Clamp(index, 0, _eventButtons.Length - 1);
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
    
    
    public void Update_TextSize(TextMeshProUGUI updateText)
    {
        float textSize = 1;
        int lineCount = updateText.textInfo.lineCount;

        for (int i = 0; i < _maxTextSize * 10; i++)
        {
            textSize += 0.1f;
            
            updateText.fontSize = textSize;
            updateText.ForceMeshUpdate();
            
            lineCount = updateText.textInfo.lineCount;

            if (lineCount <= 1 && textSize < _maxTextSize) continue;
            break;
        }

        updateText.fontSize = Mathf.Round((textSize - 0.1f) * 10f) / 10f;
        updateText.ForceMeshUpdate();
    }
    public void Update_TextSize()
    {
        foreach (Menu_EventButton eventButton in _eventButtons)
        {
            if (eventButton.buttonText == null) continue;
            Update_TextSize(eventButton.buttonText);
        }
    }
    
    
    public void Select_Action()
    {
        if (TransitionCanvas_Controller.instance.coroutine != null) return;
        if (_eventButtons.Length <= 0) return;

        _eventButtons[_currentIndex].actionEvent?.Invoke();
        OnAction?.Invoke();
    }
    public void Select_Action(int index)
    {
        if (_currentIndex != index)
        {
            Navigate_ButtonIndex(index);
            return;
        }
        Select_Action();
    }

    
    public void Exit()
    {
        OnExit?.Invoke();
    }
}
