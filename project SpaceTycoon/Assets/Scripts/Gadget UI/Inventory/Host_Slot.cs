using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Host_Slot : MonoBehaviour
{
    public Host_System system;

    public bool slotSelected = false;
    public bool hasItem = false;

    public Item_Info currentItem;
    public int currentAmount;

    public GameObject slotSelectHighlighter, toolTipPanel, moveButton;
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
        amountText.text = currentAmount.ToString();
    }
    public void Stack_Slot(int additionalAmount)
    {
        currentAmount += additionalAmount;
        amountText.text = currentAmount.ToString();
    }

    private void Select_Slot()
    {
        if (system.guestSystem_Connected())
        {
            system.guestSystem.DeSelect_All_Slots();
        }
        system.DeSelect_All_Slots();
        slotSelected = true;
        slotSelectHighlighter.SetActive(true);
        toolTipPanel.SetActive(true);

        var guestSystem = system.guestSystem;
        if (system.guestSystem_Connected())
        {
            if (guestSystem.Slot_Available() || guestSystem.Stack_Available(currentItem))
            {
                moveButton.SetActive(true);
            }
        }
    }
    public void DeSelect_Slot()
    {
        slotSelected = false;
        slotSelectHighlighter.SetActive(false);
        toolTipPanel.SetActive(false);
        moveButton.SetActive(false);
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
    public void Move_Slot()
    {
        var currentItem = this.currentItem;
        int currentAmount = this.currentAmount;
        Empty_Slot();
        system.guestSystem.Craft_Item(currentItem, currentAmount);
        DeSelect_Slot();
    }
}
