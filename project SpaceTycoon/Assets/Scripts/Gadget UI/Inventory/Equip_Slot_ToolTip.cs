using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Equip_Slot_ToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Equip_Slot thisEquipSlot;
    
    public GameObject[] panels;
    public GameObject moveButton, unEquipButton;
    public RectTransform[] rectTransforms;
    public Image[] itemIcons;
    public Text[] itemNames;
    public Text[] itemDescriptions;

    private bool timerStart;
    private float timer = 0, onHoverTime = 0.5f;

    private void Update()
    {
        ToolTip_Timer();
        Hover_Show_ToolTip();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (thisEquipSlot.hasItem)
        {
            timerStart = true;
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        panels[0].SetActive(false);
        timerStart = false;
    }

    private void Update_ToolTip_Info()
    {
        if (thisEquipSlot.hasItem)
        {
            var x = thisEquipSlot.currentItem;
            itemIcons[0].sprite = x.itemIcon;
            itemNames[0].text = x.itemName;
            itemDescriptions[0].text = x.itemDescription;
        }
    }
    public void Update_Interactive_ToolTip_Info()
    {
        var x = thisEquipSlot.currentItem;
        itemIcons[1].sprite = x.itemIcon;
        itemNames[1].text = x.itemName;
        itemDescriptions[1].text = x.itemDescription;

        panels[0].SetActive(false);
        Check_InteractableToolTip_inScreen();
        panels[1].SetActive(true);
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
        if (timer >= onHoverTime && !thisEquipSlot.system.slotSelected)
        {
            Update_ToolTip_Info();
            Check_ToolTip_inScreen();
            panels[0].SetActive(true);
        }
    }

    // stay inside screen
    private bool Is_Slot_RightSide()
    {
        var slotRT = thisEquipSlot.slotRT.anchoredPosition.x;
        if (slotRT == 177.3f) { return true; }
        else return false;
    }
    private void Check_ToolTip_inScreen()
    {
        if (Is_Slot_RightSide())
        {
            rectTransforms[0].anchoredPosition = new Vector2(-40, 90);
        }
    }
    private void Check_InteractableToolTip_inScreen()
    {
        if (Is_Slot_RightSide())
        {
            rectTransforms[1].anchoredPosition = new Vector2(-40, 105);
        }
    }
}
