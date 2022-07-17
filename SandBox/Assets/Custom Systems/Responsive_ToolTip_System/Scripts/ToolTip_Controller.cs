using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ToolTip_Controller : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string word;
    [TextArea(10, 15)]
    public string definition;
    
    public GameObject toolTipPanel;
    public Text titleText;
    public Text infoText;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Show_ToolTip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Hide_ToolTip();
    }

    private void Text_Update()
    {
        titleText.text = word;
        infoText.text = definition;
    }

    private void Show_ToolTip()
    {
        Text_Update();
        toolTipPanel.SetActive(true);
    }

    private void Hide_ToolTip()
    {
        toolTipPanel.SetActive(false);
    }

}
