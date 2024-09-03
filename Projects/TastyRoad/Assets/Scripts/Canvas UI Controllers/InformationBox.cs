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
        _defaultHeight = _rect.anchoredPosition.y;
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
}
