using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Guest_Slot : MonoBehaviour
{
    [HideInInspector]
    public Guest_System system;
    public RectTransform slotRT;
    public Guest_Slot_ToolTip toolTip;

    public bool slotSelected = false;
    public bool hasItem = false;

    [HideInInspector]
    public Item_Info currentItem;
    public int currentAmount;
    public float currentDurability;

    public Slider durabilitySlider;
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

        Durability_Slider_Activation_Check();
    }
    public void Assign_Slot(Item_Info itemInfo, int itemAmount)
    {
        hasItem = true;
        currentItem = itemInfo;
        currentAmount = itemAmount;
        itemSprite.sprite = itemInfo.itemIcon;
        itemSprite.color = Color.white;
        amountText.text = itemAmount.ToString();

        Durability_Slider_Activation_Check();
    }
    public void Stack_Slot(int additionalAmount)
    {
        currentAmount += additionalAmount;
        amountText.text = currentAmount.ToString();
    }
    public void Trash_Item()
    {
        // subtract amount
        currentAmount -= 1;

        if (currentAmount >= 1)
        {
            amountText.text = currentAmount.ToString();
        }
        else if (currentAmount <= 0)
        {
            // empty the slot
            Empty_Slot();
            DeSelect_Slot();
        }
    }
    public void Trash_All_Item()
    {
        Empty_Slot();
        DeSelect_Slot();
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
        if (system.hostSystem.inventoryMenu.activeSelf)
        {
            if (equipSystem.Slot_Available(currentItem.itemType) || equipSystem.Stack_Available(currentItem))
            {
                toolTip.equipButton.SetActive(true);
            }
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
        system.hostSystem.Craft_Item(false, 2, currentItem, itemAmount, currentDurability);
        DeSelect_Slot();
    }
    public void Equip_Slot()
    {
        var currentItem = this.currentItem;
        int itemAmount = currentAmount;
        Empty_Slot();
        system.hostSystem.equipSystem.Craft_Item(false, 2, currentItem, itemAmount, currentDurability);
        DeSelect_Slot();
    }

    // durability sliders
    public void Durability_Slider_Activation_Check()
    {
        if (hasItem && currentItem.itemMaxAmount == 1)
        {
            durabilitySlider.gameObject.SetActive(true);
            durabilitySlider.maxValue = currentItem.itemMaxDurability;
            durabilitySlider.value = currentDurability;
        }
        else
        {
            durabilitySlider.gameObject.SetActive(false);
        }
    }
}