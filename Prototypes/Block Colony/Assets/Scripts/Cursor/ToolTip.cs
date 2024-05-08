using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ToolTip : MonoBehaviour
{
    [Header("")]
    [SerializeField] private RectTransform _cursorPoint;

    [Header("")]
    [SerializeField] private RectTransform _panel;
    [SerializeField] private TextMeshProUGUI _descriptionText;

    [Header("")]
    [SerializeField] private float _hoverTime;

    private Coroutine _toggleCoroutine;


    // MonoBehaviour
    private void Start()
    {
        Toggle(false);
    }

    private void Update()
    {
        Vector2 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        _cursorPoint.transform.position = mousePos;
    }


    //
    public void Toggle(bool toggleOn)
    {
        if (toggleOn == false)
        {
            _cursorPoint.gameObject.SetActive(false);
            if (_toggleCoroutine != null) StopCoroutine(_toggleCoroutine);
            return;
        }

        _toggleCoroutine = StartCoroutine(Toggle_Coroutine());
    }
    public IEnumerator Toggle_Coroutine()
    {
        yield return new WaitForSeconds(_hoverTime);

        _cursorPoint.gameObject.SetActive(true);
        PanelPosition_Update();
    }

    public void DescriptionText_Update(string description)
    {
        _descriptionText.text = description;
    }


    private bool Mouse_LeftSide()
    {
        Vector3 mousePosition = Input.mousePosition;
        bool isOnLeftSide = mousePosition.x < Screen.width / 2f;

        return isOnLeftSide;
    }

    private void PanelPosition_Update()
    {
        if (Mouse_LeftSide())
        {
            _panel.anchoredPosition = new Vector2(210, _panel.anchoredPosition.y);
        }
        else
        {
            _panel.anchoredPosition = new Vector2(-10, _panel.anchoredPosition.y);
        }
    }
}
