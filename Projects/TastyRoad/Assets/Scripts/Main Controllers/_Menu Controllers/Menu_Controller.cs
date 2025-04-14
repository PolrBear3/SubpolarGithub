using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Menu_EventButton
{
    [SerializeField] private Image _button;
    public Image button => _button;
    
    [SerializeField] private UnityEvent _actionEvent;
    public UnityEvent actionEvent => _actionEvent;
    
    private Vector2 _defaultPosition;
    public Vector2 defaultPosition => _defaultPosition;


    public void Set_DefaultPosition()
    {
        _defaultPosition = button.rectTransform.anchoredPosition;
    }
}

public class Menu_Controller : MonoBehaviour
{
    [Header("")]
    [SerializeField] private Image _menuPanel;
    public Image menuPanel => _menuPanel;
    
    [SerializeField] private Menu_EventButton[] _eventButtons;
    public Menu_EventButton[] eventButtons => _eventButtons;

    
    private int _currentIndex;
    public int currentIndex => _currentIndex;

    public Action OnNavigate;
    public Action OnAction;


    // MonoBehaviour
    public void Awake()
    {
        
    }

    public void Start()
    {
        foreach (Menu_EventButton button in _eventButtons)
        {
            button.Set_DefaultPosition();
        }
        
        Toggle_Menu(true);
    }

    public void OnDestroy()
    {
        Input_Controller input = Input_Controller.instance;

        input.OnCursorControl -= Navigate_Action;
        input.OnSelect -= Select_Action;
    }


    // Data Control
    public void Set_CurrentIndex(int indexNum)
    {
        _currentIndex = Mathf.Clamp(indexNum, 0, _eventButtons.Length - 1);
    }


    // Menu Control
    public bool Menu_Toggled()
    {
        return _menuPanel.gameObject.activeSelf;
    }
    
    public void Toggle_Menu(bool toggle)
    {
        Input_Controller input = Input_Controller.instance;

        if (_menuPanel != null) _menuPanel.gameObject.SetActive(toggle);

        if (toggle)
        {
            input.Update_ActionMap(1);

            Debug.Log(input.playerInput.currentActionMap);

            input.OnCursorControl += Navigate_Action;
            input.OnSelect += Select_Action;

            NavigateUpdate_EventButtons();
            return;
        }

        input.Update_ActionMap(0);

        input.OnCursorControl -= Navigate_Action;
        input.OnSelect -= Select_Action;
    }


    private void Navigate_Action(Vector2 direction)
    {
        int actionCount = _eventButtons.Length;
        if (actionCount <= 0) return;

        float value = direction.x + direction.y;
        _currentIndex = (_currentIndex + (int)value + actionCount) % actionCount;

        NavigateUpdate_EventButtons();
        OnNavigate?.Invoke();
        
        // sound
        Audio_Controller.instance.Play_OneShot(gameObject, 1);
    }

    public void NavigateUpdate_EventButtons()
    {
        for (int i = 0; i < _eventButtons.Length; i++)
        {
            RectTransform buttonTransform = _eventButtons[i].button.rectTransform;
            
            if (i != currentIndex)
            {
                buttonTransform.localPosition = _eventButtons[i].defaultPosition;
                continue;
            }

            buttonTransform.localPosition = Vector2.zero;
        }
    }
    
    
    private void Select_Action()
    {
        if (_eventButtons.Length <= 0) return;

        _eventButtons[_currentIndex].actionEvent?.Invoke();
        OnAction?.Invoke();
    }
    
    
    // Main Menu
    public void Start_InGame()
    {
        StartCoroutine(Start_InGame_Coroutine());
    }
    private IEnumerator Start_InGame_Coroutine()
    {
        TransitionCanvas_Controller transition = TransitionCanvas_Controller.instance;
        transition.CloseScene_Transition();

        while (transition.transitionPlaying) yield return null;

        SceneManager.LoadScene(1);
        yield break;
    }
}
