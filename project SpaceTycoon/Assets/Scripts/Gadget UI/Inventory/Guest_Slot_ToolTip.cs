using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Guest_Slot_ToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Guest_Slot thisGuestSlot;

    public GameObject toolTipPanel, interactableToolTip, moveButton;
    public Image itemIcon, itemIcon2;
    public Text itemName, itemName2, itemDescription, itemDescription2;

    private bool timerStart;
    private float timer = 0, onHoverTime = 0.5f;

    private void Update()
    {
        ToolTip_Timer();
        Hover_Show_ToolTip();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (thisGuestSlot.hasItem)
        {
            timerStart = true;
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        toolTipPanel.SetActive(false);
        timerStart = false;
    }

    private void Update_ToolTip_Info()
    {
        var x = thisGuestSlot.currentItem;
        itemIcon.sprite = x.itemIcon;
        itemName.text = x.itemName;
        itemDescription.text = x.itemDescription;
    }
    public void Update_Interactive_ToolTip_Info()
    {
        var x = thisGuestSlot.currentItem;
        itemIcon2.sprite = x.itemIcon;
        itemName2.text = x.itemName;
        itemDescription2.text = x.itemDescription;
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
    private void Hover_Show_ToolTip()
    {
        if (timer >= onHoverTime && !thisGuestSlot.system.slotSelected)
        {
            Update_ToolTip_Info();
            toolTipPanel.SetActive(true);
        }
    }
}
