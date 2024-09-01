using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        _defaultHeight = _rect.anchoredPosition.y;
        // _rect.gameObject.SetActive(false);
    }


    // Panel Layout Control
    public void Flip()
    {
        float currentX = _rect.anchoredPosition.x;
        _rect.anchoredPosition = new Vector2(currentX * -1, _rect.anchoredPosition.y);
    }
}
