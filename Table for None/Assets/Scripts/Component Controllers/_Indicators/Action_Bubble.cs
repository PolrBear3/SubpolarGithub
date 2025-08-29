using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;

[System.Serializable]
public class ActionBubble_Data
{
    [SerializeField] private Sprite _iconSprite;
    public Sprite iconSprite => _iconSprite;
    
    [SerializeField] private string _bubbleInfo;
    public string bubbleInfo => _bubbleInfo;
    
    [SerializeField] private LocalizedString _localizedInfo;
    
    // New
    public ActionBubble_Data(Sprite iconSprite, string bubbleInfo)
    {
        _iconSprite = iconSprite;
        _bubbleInfo = bubbleInfo;
    }
    
    // Get
    public string LocalizedInfo()
    {
        if (_localizedInfo == null) return _bubbleInfo;
        if (string.IsNullOrEmpty(_localizedInfo.TableReference) && string.IsNullOrEmpty(_localizedInfo.TableEntryReference)) return _bubbleInfo;
        
        return _localizedInfo.GetLocalizedString();
    }
}

public class Action_Bubble : MonoBehaviour
{
    [Space(20)]
    [SerializeField] private GameObject _toggle;
    [SerializeField] private GameObject _leftBubble;
    [SerializeField] private GameObject _rightBubble;

    [Space(20)]
    [SerializeField] private SpriteRenderer _leftIcon;
    public SpriteRenderer leftIcon => _leftIcon;

    [SerializeField] private SpriteRenderer _rightIcon;
    public SpriteRenderer rightIcon => _rightIcon;

    [Space(20)] 
    [SerializeField] private ActionKey[] _actionKeys;
    [SerializeField] private float _toggleHeight;

    [Space(20)]
    [SerializeField] private bool _loadToggleDatas;
    
    [SerializeField] private ActionBubble_Data[] _bubbleDatas;
    public ActionBubble_Data[] bubbleDatas => _bubbleDatas;

    [Space(20)] 
    [SerializeField] private UI_EffectController _effectController;


    private float _defaultHeight;

    private bool _bubbleOn;
    public bool bubbleOn => _bubbleOn;
    
    private List<ActionBubble_Data> _indicatorToggleDatas = new();
    

    // MonoBehaviour
    private void Awake()
    {
        _defaultHeight = transform.localPosition.y;
    }

    private void Start()
    {
        Toggle(false);
        Load_IndicatorToggleDatas();
    }


    // Show and Hide
    public void Toggle(bool toggleOn)
    {
        InteractIndicator_Controller indicator = InteractIndicator_Controller.instance;
        
        if (toggleOn == false)
        {
            _bubbleOn = false;
            _toggle.SetActive(false);
            
            indicator.Toggle(this, null);
            return;
        }
        
        Update_Bubble(_leftIcon.sprite, _rightIcon.sprite);
        _effectController.Update_Scale(gameObject);

        foreach (ActionKey actionKey in _actionKeys)
        {
            actionKey.Set_CurrentKey();
        }

        if (_indicatorToggleDatas.Count <= 0) return;
        indicator.Toggle(this, _indicatorToggleDatas);
    }


    // Interact Indicator
    private void Load_IndicatorToggleDatas()
    {
        if (_loadToggleDatas == false) return;
        
        for (int i = 0; i < _bubbleDatas.Length; i++)
        {
            _indicatorToggleDatas.Add(_bubbleDatas[i]);
        }
    }
    
    public void Set_IndicatorToggleDatas(List<ActionBubble_Data> setDatas)
    {
        _indicatorToggleDatas.Clear();
        
        if (setDatas == null) return;
        _indicatorToggleDatas = setDatas;
    }
    

    // Icon Sprite
    public void Set_Bubble(Sprite leftIcon, Sprite rightIcon)
    {
        if (leftIcon == null) return;
        _leftIcon.sprite = leftIcon;

        if (rightIcon == null) return;
        _rightIcon.sprite = rightIcon;
    }

    public void Update_Bubble(Sprite leftIcon, Sprite rightIcon)
    {
        // turn off if bubble is on || no sprites are assigned
        if (_toggle.activeSelf == true || leftIcon == null)
        {
            Toggle(false);
            return;
        }

        // toggle on
        _bubbleOn = true;

        _toggle.SetActive(true);

        // left bubble toggle on
        _leftBubble.SetActive(true);
        _leftIcon.sprite = leftIcon;

        // right bubble toggle
        if (rightIcon != null)
        {
            _rightBubble.SetActive(true);
            _rightIcon.sprite = rightIcon;
        }
        else
        {
            _rightBubble.SetActive(false);
        }

        Update_BubblePosition();
    }

    public void Empty_Bubble()
    {
        _leftIcon.sprite = null;
        _rightIcon.sprite = null;
    }


    // Position
    private void Update_BubblePosition()
    {
        Vector2 leftPos = _leftBubble.transform.localPosition;
        Vector2 rightPos = _rightBubble.transform.localPosition;

        if (_rightIcon.sprite == null)
        {
            _leftBubble.transform.localPosition = new Vector2(0f, leftPos.y);
        }
        else
        {
            _leftBubble.transform.localPosition = new Vector2(-0.4f, leftPos.y);
            _rightBubble.transform.localPosition = new Vector2(0.4f, rightPos.y);
        }
    }


    public void Toggle_Height(bool toggle)
    {
        if (toggle)
        {
            transform.localPosition = new Vector2(transform.localPosition.x, _toggleHeight);
            return;
        }

        transform.localPosition = new Vector2(transform.localPosition.x, _defaultHeight);
    }

    public void Adjust_Height(float height)
    {
        transform.localPosition = new Vector2(transform.localPosition.x, height);
    }
}