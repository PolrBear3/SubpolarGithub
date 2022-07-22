using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCraftTable_ToolTip : MonoBehaviour
{
    public GameObject toolTipPanel;
    public RectTransform toolTipPanelRT;
    public RectTransform[] toolTipPositions;

    private void Update_ToolTip_Info(Item_Info itemInfo)
    {
        // toolTip sprite, name, description
    }
    private void Set_ToolTip_Position(int rowNum)
    {
        toolTipPanelRT.anchoredPosition = toolTipPositions[rowNum].anchoredPosition;
    }

    public void ToolTip_On(Item_Info item_Info, int rowNum)
    {
        Update_ToolTip_Info(item_Info);
        Set_ToolTip_Position(rowNum);
        toolTipPanel.SetActive(true);
    }
    public void ToolTip_Off()
    {
        toolTipPanel.SetActive(false);
    }
}
