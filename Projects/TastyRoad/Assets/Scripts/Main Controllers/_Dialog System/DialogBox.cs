using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogBox : MonoBehaviour
{
    [Header("")]
    [SerializeField] private Image _iconImage;
    [SerializeField] private RectTransform _newIndication;

    [Header("")]
    [SerializeField] private Image _infoPanel;
    [SerializeField] private TextMeshProUGUI _infoText;

    private Vector2 _defaultPosition;
    private bool _panelOpened;


    // MonoBehaviour


    // Data Set
    public void Update_Box(DialogData data)
    {
        _defaultPosition = _infoPanel.rectTransform.localPosition;

        _iconImage.sprite = data.icon;
        _infoText.text = data.info;

        InfoPanel_RepositionUpdate();
        InfoPanel_Toggle(false);
    }

    public void UpdateIcon_CenterPosition(Vector2 centerPosition)
    {
        _iconImage.rectTransform.localPosition = centerPosition;
    }


    // Basic Panel Control
    public void InfoPanel_Toggle(bool toggleOn)
    {
        _infoPanel.gameObject.SetActive(toggleOn);
        _panelOpened = toggleOn;

        if (_panelOpened == false) return;
        _newIndication.gameObject.SetActive(false);
    }

    public void InfoPanel_RepositionUpdate()
    {
        _infoText.ForceMeshUpdate();
        Canvas.ForceUpdateCanvases();

        float yPos = _infoText.textInfo.lineCount * -12.5f;

        _infoPanel.rectTransform.localPosition = new Vector2(_defaultPosition.x, _defaultPosition.y + yPos);
    }
}
