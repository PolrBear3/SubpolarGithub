using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemCraftTable_Button : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ItemCraftTable controller;
    public ItemCraftTable_ToolTip toolTip;

    public int rowNum;
    public Item_Info itemInfo;

    bool timerStart;
    float timer = 0;
    float onHoverTime = 0.5f;

    private void Update()
    {
        ToolTip_Timer();
        Show_ToolTip();
    }

    public void Open_OptionPanel_forThisItem()
    {
        controller.Open_Item_OptionPanel(itemInfo);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        timerStart = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        toolTip.ToolTip_Off();
        timerStart = false;
    }

    private void ToolTip_Timer()
    {
        if (timerStart)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0;
        }
    }
    private void Show_ToolTip()
    {
        if (timer >= onHoverTime)
        {
            toolTip.ToolTip_On(itemInfo, rowNum);
        }
    }
}
