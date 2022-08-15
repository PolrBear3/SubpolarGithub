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

    public GameObject slotSelectHighlighter, moveButton;
    public Image itemSprite;
    public Text amountText;

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

        system.ingredientsCheckSystem.CountAdd_Ingredients(itemInfo, itemAmount);
    }
    public void Stack_Slot(int additionalAmount)
    {
        itemAmount += additionalAmount;
        amountText.text = itemAmount.ToString();

        system.ingredientsCheckSystem.CountAdd_Ingredients(currentItem, additionalAmount);
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

        var slots_System = system.guestSystem;
        if (system.guestSystem_Connected())
        {
            if (slots_System.Slot_Available() || slots_System.Stack_Available(currentItem))
            {
                moveButton.SetActive(true);
            }
        }
    }
    public void DeSelect_Slot()
    {
        slotSelected = false;
        slotSelectHighlighter.SetActive(false);
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
        int itemAmount = this.itemAmount;
        Empty_Slot();
        system.guestSystem.Craft_Item(currentItem, itemAmount);
        system.ingredientsCheckSystem.CountSubtract_Ingredients(currentItem, itemAmount);
        DeSelect_Slot();
    }
}
