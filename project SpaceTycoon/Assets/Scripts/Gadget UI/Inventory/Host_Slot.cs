using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Host_Slot : MonoBehaviour
{
    public Host_System system;
    public RectTransform slotRT;
    public Host_Slot_ToolTip toolTip;

    public bool slotSelected = false;
    public bool hasItem = false;

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
        amountText.text = currentAmount.ToString();

        Durability_Slider_Activation_Check();
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
        system.equipSystem.DeSelect_All_Slots();

        system.DeSelect_All_Slots();
        slotSelected = true;
        system.slotSelected = true;
        slotSelectHighlighter.SetActive(true);
        toolTip.Update_Interactive_ToolTip_Info();

        var guestSystem = system.guestSystem;
        if (system.guestSystem_Connected())
        {
            if (guestSystem.Slot_Available() || guestSystem.Stack_Available(currentItem))
            {
                toolTip.moveButton.SetActive(true);
            }
        }

        var equipSystem = system.equipSystem;
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
        int currentAmount = this.currentAmount;
        Empty_Slot();
        system.guestSystem.Craft_Item(false, 1, currentItem, currentAmount, currentDurability);
        DeSelect_Slot();
    }
    public void Equip_Slot()
    {
        var currentItem = this.currentItem;
        int currentAmount = this.currentAmount;
        Empty_Slot();
        system.equipSystem.Craft_Item(false, 1, currentItem, currentAmount, currentDurability);
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
