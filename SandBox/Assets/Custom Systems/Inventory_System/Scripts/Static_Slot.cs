using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Static_Slot : MonoBehaviour
{
    public Static_Slots_System system;

    public bool slotSelected = false;
    public bool hasItem = false;

    public Item_Info currentItem;
    public int itemAmount;
    public int moveAmount;

    public GameObject slotSelectHighlighter, moveButton, amountScroller;
    public Scrollbar scrollBar;
    public Image itemSprite;
    public Text amountText;
    public Text moveAmountText;

    public void Empty_Slot()
    {
        itemAmount = 0;
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
        this.itemAmount = itemAmount;
        itemSprite.sprite = itemInfo.itemSprite;
        itemSprite.color = Color.white;
        amountText.text = itemAmount.ToString();
    }
    public void Stack_Slot(int additionalAmount)
    {
        itemAmount += additionalAmount;
        amountText.text = itemAmount.ToString();
    }

    private void Select_Slot()
    {
        system.DeSelect_All_Slots();
        slotSelected = true;
        slotSelectHighlighter.SetActive(true);

        var slots_System = system.guestSystem;
        if (slots_System.Slot_Available() || slots_System.Stack_Available(currentItem))
        {
            amountScroller.SetActive(true);
            moveButton.SetActive(true);
        }

        Set_Scroll_System();
    }
    public void DeSelect_Slot()
    {
        slotSelected = false;
        slotSelectHighlighter.SetActive(false);
        moveButton.SetActive(false);
        amountScroller.SetActive(false);
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

    private void Set_Scroll_System()
    {
        scrollBar.value = float.MaxValue;
        moveAmount = itemAmount;
        moveAmountText.text = moveAmount.ToString();
    }
    public void Scroll_System_Update()
    {
        var moveValue = scrollBar.value * itemAmount;
        moveAmount = (int)moveValue;
        moveAmountText.text = moveAmount.ToString();

        if (moveAmount == 0)
        {
            moveButton.SetActive(false);
        }
        else
        {
            moveButton.SetActive(true);
        }
    }
    public void Move_Slot()
    {
        var currentItem = this.currentItem;
        int remainingAmount = itemAmount - moveAmount;

        var slots_System = system.guestSystem;
        if (moveAmount > 0 && slots_System.Slot_Available() || slots_System.Stack_Available(currentItem))
        {
            Empty_Slot();
            if (remainingAmount > 0)
            {
                Assign_Slot(currentItem, remainingAmount);
            }
            system.guestSystem.Stack_Item(currentItem, moveAmount);
        }

        DeSelect_Slot();
    }
}
