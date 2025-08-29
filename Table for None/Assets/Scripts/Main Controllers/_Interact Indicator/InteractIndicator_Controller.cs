using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class InteractIndicator_Component
{
    [SerializeField] private RectTransform _iconBox;
    public RectTransform iconBox => _iconBox;
    
    [SerializeField] private Image _iconImage;
    public Image iconImage => _iconImage;

    [Space(10)] 
    [SerializeField] private RectTransform _infoBox;
    public RectTransform infoBox => _infoBox;
    
    [SerializeField] private TextMeshProUGUI _infoText;
    public TextMeshProUGUI infoText => _infoText;

    public float iconBoxPos;
    public float infoBoxPos;
}

public class InteractIndicator_Controller : MonoBehaviour
{
    public static InteractIndicator_Controller instance;


    [Space(20)] 
    [SerializeField] private UI_EffectController _uiEffect;
    
    [Space(20)]
    [SerializeField] private RectTransform _interactIndicator;
    [SerializeField] private InteractIndicator_Component _mainComponent;
    
    [Space(20)]
    [SerializeField] private RectTransform _actionBubbleIndicator;
    [SerializeField] private InteractIndicator_Component[] _bubbleComponents;

    [Space(20)] 
    [SerializeField] [Range(0, 100)] private float _infoShowtime;


    private Coroutine _coroutine;
    private Action_Bubble _indicatingBubble;
    
    
    // UnityEngine
    private void Awake()
    {
        instance = this;

        foreach (InteractIndicator_Component component in _bubbleComponents)
        {
            component.iconBoxPos = component.iconBox.anchoredPosition.x;
            component.infoBoxPos = component.infoBox.anchoredPosition.x;
        }
        
        Trigger(null, null);
        Toggle(null, null);
    }

    private void Start()
    {
        Toggle(Toggle_Available());
        
        // subscriptions
        Input_Controller.instance.OnActionMapUpdate += Toggle;
        Cutscene_Controller.instance.OnToggle += Toggle;
    }

    private void OnDestroy()
    {
        Cutscene_Controller.instance.OnToggle -= Toggle;
    }


    // Trigger
    private bool Toggle_Available()
    {
        bool inGame = Input_Controller.instance.Current_ActionMapNum() == 0 && _mainComponent.iconImage.sprite != null;
        bool cutscenePlaying = Cutscene_Controller.instance.coroutine != null;
        
        return inGame && cutscenePlaying == false;
    }
    
    private void Toggle(bool toggle)
    {
        if (toggle == false || _mainComponent.iconImage.sprite == null)
        {
            _mainComponent.iconBox.gameObject.SetActive(false);
            _mainComponent.infoBox.gameObject.SetActive(false);
            
            return;
        }

        Trigger(_mainComponent.iconImage.sprite, _mainComponent.infoText.text);
    }
    private void Toggle()
    {
        Toggle(Toggle_Available());
    }
    
    
    public void Trigger(Sprite icon, string info)
    {
        _mainComponent.iconBox.gameObject.SetActive(icon != null);

        if (icon == null)
        {
            _mainComponent.iconImage.sprite = null;
            
            _mainComponent.infoBox.gameObject.SetActive(false);
            return;
        }
        
        _mainComponent.iconImage.sprite = icon;
        _mainComponent.infoText.text = info;

        if (info == null || info == String.Empty)
        {
            _mainComponent.infoBox.gameObject.SetActive(false);
            return;
        }
        
        _actionBubbleIndicator.gameObject.SetActive(false);
        Toggle_InfoBox();
        
        _uiEffect.Update_Scale(_interactIndicator);
    }

    private void Toggle_InfoBox()
    {
        Cancel_Coroutine();
        _coroutine = StartCoroutine(InfoBox_Coroutine());
    }
    private IEnumerator InfoBox_Coroutine()
    {
        _mainComponent.infoBox.gameObject.SetActive(true);
       
        yield return new WaitForSeconds(_infoShowtime);
        
        _mainComponent.infoBox.gameObject.SetActive(false);
        _coroutine = null;
    }


    private void Cancel_Coroutine()
    {
        if (_coroutine == null) return;
        
        StopCoroutine(_coroutine);
        _coroutine = null;
    }
    
    
    // Action Bubble Trigger
    public void Toggle(Action_Bubble targetBubble, List<ActionBubble_Data> bubbleDatas)
    {
        if (_indicatingBubble != null && _indicatingBubble != targetBubble) return;
        _indicatingBubble = targetBubble;
        
        bool toggle = bubbleDatas != null && bubbleDatas.Count > 0;

        if (_actionBubbleIndicator != null) _actionBubbleIndicator.gameObject.SetActive(toggle);
        if (_interactIndicator != null) _interactIndicator.gameObject.SetActive(!toggle);

        if (toggle == false)
        {
            _indicatingBubble = null;
            return;
        }

        int updateCount = 0;
        for (int i = 0; i < _bubbleComponents.Length; i++)
        {
            bool componentToggle = i <= bubbleDatas.Count - 1 && bubbleDatas[i].iconSprite != null;

            string bubbleInfo = componentToggle ? bubbleDatas[i].bubbleInfo : null;
            bool hasInfo = bubbleInfo != null && bubbleInfo != string.Empty;

            _bubbleComponents[i].iconBox.gameObject.SetActive(componentToggle);
            _bubbleComponents[i].infoBox.gameObject.SetActive(componentToggle && hasInfo);
            
            if (componentToggle == false) break;
            
            _bubbleComponents[i].iconImage.sprite = bubbleDatas[i].iconSprite;
            _bubbleComponents[i].infoText.text = bubbleDatas[i].bubbleInfo;
            
            updateCount++;
        }

        bool isDefaultPos = updateCount > 1;
        foreach (InteractIndicator_Component component in _bubbleComponents)
        {
            float iconBoxPos = isDefaultPos ? component.iconBoxPos : 0f;
            float textBoxPos = isDefaultPos ? component.infoBoxPos : 0f;
            
            component.iconBox.anchoredPosition = new Vector2(iconBoxPos, component.iconBox.anchoredPosition.y);
            component.infoBox.anchoredPosition = new Vector2(textBoxPos, component.infoBox.anchoredPosition.y);
        }
    }
}