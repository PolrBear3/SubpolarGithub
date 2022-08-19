using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Host_Slot_ToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Host_Slot thisHostSlot;
    public ToolTip_ItemType_Icon itemTypeIconSystem;

    public GameObject[] panels;
    public GameObject moveButton, equipButton;
    public RectTransform[] rectTransforms;
    public Image[] itemIcons;
    public Image[] itemTypeIcons;
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
        timerStart = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        panels[0].SetActive(false);
        timerStart = false;
    }

    private void Update_ToolTip_Info()
    {
        if (thisHostSlot.hasItem)
        {
            var x = thisHostSlot.currentItem;
            itemIcons[0].sprite = x.UIitemIcon;
            itemNames[0].text = x.itemName;
            itemDescriptions[0].text = x.itemDescription;
            itemTypeIconSystem.Set_ItemType_Icon(thisHostSlot.currentItem.itemType);
            itemTypeIcons[0].sprite = itemTypeIconSystem.currentItemTypeIcon;
        }
    }
    public void Update_Interactive_ToolTip_Info()
    {
        var x = thisHostSlot.currentItem;
        itemIcons[1].sprite = x.UIitemIcon;
        itemNames[1].text = x.itemName;
        itemDescriptions[1].text = x.itemDescription;
        itemTypeIconSystem.Set_ItemType_Icon(thisHostSlot.currentItem.itemType);
        itemTypeIcons[1].sprite = itemTypeIconSystem.currentItemTypeIcon;

        panels[0].SetActive(false);
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
        if (timer >= onHoverTime && !panels[1].activeSelf && thisHostSlot.hasItem)
        {
            Update_ToolTip_Info();
            panels[0].SetActive(true);
        }
    }
}
