using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Guest_Slot : MonoBehaviour
{
    // system type num > 2

    public Guest_System system;
    public RectTransform slotRT;
    public Guest_Slot_ToolTip toolTip;

    public bool slotSelected = false;
    public bool hasItem = false;

    public Item_Info currentItem;
    public int currentAmount;

    public GameObject slotSelectHighlighter;
    public Image itemSprite;
    public Text amountText;

    public void Empty_Slot()
    {
        currentAmount = 0;
        hasItem = false;
        currentItem = null;
        itemSprite.sprite = null;
        itemSprite.color = Color.clear;
        amountText.text = "";
    }
    public void Assign_Slot(Item_Info itemInfo, int itemAmount)
    {
        hasItem = true;
        currentItem = itemInfo;
        currentAmount = itemAmount;
        itemSprite.sprite = itemInfo.itemIcon;
        itemSprite.color = Color.white;
        amountText.text = itemAmount.ToString();
    }
    public void Stack_Slot(int additionalAmount)
    {
        currentAmount += additionalAmount;
        amountText.text = currentAmount.ToString();
    }

    private void Select_Slot()
    {
        if (system.hostSystem_Connected())
        {
            system.hostSystem.DeSelect_All_Slots();
        }

        system.DeSelect_All_Slots();
        slotSelected = true;
        system.slotSelected = true;
        slotSelectHighlighter.SetActive(true);
        toolTip.Update_Interactive_ToolTip_Info();

        var hostSystem = system.hostSystem;
        if (system.hostSystem.inventoryMenu.activeSelf)
        {
            if (hostSystem.Slot_Available() || hostSystem.Stack_Available(currentItem))
            {
                toolTip.moveButton.SetActive(true);
            }
        }

        var equipSystem = system.hostSystem.equipSystem;
        if (equipSystem.Slot_Available(currentItem.itemType) || equipSystem.Stack_Available(currentItem))
        {
            toolTip.equipButton.SetActive(true);
        }
    }
    public void DeSelect_Slot()
    {
        slotSelected = false;
        system.slotSelected = false;
        slotSelectHighlighter.SetActive(false);
        toolTip.panels[1].SetActive(false);
        toolTip.moveButton.SetActive(false);
        toolTip.equipButton.SetActive(false);
    }
    public void Click_Slot()
    {
        if (!slotSelected && hasItem)
        {
            Select_Slot();
        }
        else if (slotSelected)
        {
            DeSelect_Slot();
        }
    }

    // interactive tooltip buttons
    public void Move_Slot()
    {
        var currentItem = this.currentItem;
        int itemAmount = currentAmount;
        Empty_Slot();
        system.hostSystem.Craft_Item(2, currentItem, itemAmount);
        DeSelect_Slot();
    }
    public void Equip_Slot()
    {
        var currentItem = this.currentItem;
        int itemAmount = currentAmount;
        Empty_Slot();
        system.hostSystem.equipSystem.Craft_Item(2, currentItem, itemAmount);
        DeSelect_Slot();
    }
}