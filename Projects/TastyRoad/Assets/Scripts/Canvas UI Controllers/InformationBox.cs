using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InformationBox : MonoBehaviour
{
    [Header("")]
    private RectTransform _rect;
    [SerializeField] private TMP_Text _infoText;

    private float _defaultHeight;
    private bool _defaultHeightSaved;

    [SerializeField] private float _heightIncreaseValue;


    // UnityEngine
    private void Awake()
    {
        _rect = gameObject.GetComponent<RectTransform>();
    }

    private void Start()
    {
        Set_DefalutHeight();

        gameObject.SetActive(false);
    }


    // Data Set
    public void Set_DefalutHeight()
    {
        if (_defaultHeightSaved == true) return;

        _defaultHeight = _rect.anchoredPosition.y;
        _defaultHeightSaved = true;
    }


    // Panel Layout Control
    public void Flip()
    {
        float currentX = _rect.anchoredPosition.x;
        _rect.anchoredPosition = new Vector2(currentX * -1, _rect.anchoredPosition.y);
    }
    public void Flip_toDefault()
    {
        while (_rect.anchoredPosition.x < 0) Flip();
    }


    public void Update_InfoText(string infoText)
    {
        _infoText.text = infoText.ToString();
    }

    public void Update_RectLayout()
    {
        _infoText.ForceMeshUpdate();
        LayoutRebuilder.ForceRebuildLayoutImmediate(_infoText.rectTransform);

        float lineCount = _infoText.textInfo.lineCount;
        float updateValue = _heightIncreaseValue * lineCount;
        float targetPosY = _defaultHeight + _heightIncreaseValue - updateValue;

        _rect.anchoredPosition = new Vector2(_rect.anchoredPosition.x, targetPosY);
    }


    // Text Templates
    public string CurrentAmount_Template(int amount)
    {
        return "Current Drag Amount > " + amount;
    }

    public string UIControl_Template(string action1, string action2, string hold)
    {
        // set [key] to current binding map

        string action1Key = "[Q] " + action1 + "\n";
        string action2Key = "[E] " + action2 + "\n";
        string holdKey = "[Hold Space] " + hold;

        return action1Key + action2Key + holdKey;
    }
}
