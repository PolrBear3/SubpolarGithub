using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemCraftTable_Button : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ItemCraftTable controller;
    public Item_Info itemInfo;

    public ToolTip_ItemType_Icon itemTypeIconSystem;
    public GameObject toolTipPanel;
    public bool leftSideSlot = false;
    public RectTransform toolTipPanelRT;
    public Image itemIcon, buttonItemItcon, itemTypeIcon;
    public Text itemName, itemDescription;

    bool timerStart;
    float timer = 0;
    float onHoverTime = 0.5f;

    private void Start()
    {
        Update_ToolTip_Info();
    }
    private void Update()
    {
        ToolTip_Timer();
        Show_ToolTip();
    }

    public void Open_OptionPanel_forThisItem()
    {
        controller.Open_Item_OptionPanel(itemInfo);
    }
    private void Update_ToolTip_Info()
    {
        buttonItemItcon.sprite = itemInfo.UIitemIcon;
        itemIcon.sprite = itemInfo.UIitemIcon;
        itemName.text = itemInfo.itemName;
        itemDescription.text = itemInfo.itemDescription;
        itemTypeIconSystem.Set_ItemType_Icon(itemInfo.itemType);
        itemTypeIcon.sprite = itemTypeIconSystem.currentItemTypeIcon;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        timerStart = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        toolTipPanel.SetActive(false);
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
            Update_ToolTip_Info();
            Check_ToolTip_inScreen();
            toolTipPanel.SetActive(true);
        }
    }

    // tooltip stay inside screen
    private void Check_ToolTip_inScreen()
    {
        if (leftSideSlot)
        {
            toolTipPanelRT.anchoredPosition = new Vector2(50f, 85f);
        }
    }
}
