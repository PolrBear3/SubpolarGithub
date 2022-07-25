using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public Slots_System system;

    public bool slotSelected = false;
    public bool hasItem = false;

    public Item_Info currentItem;
    public int itemAmount;

    public GameObject slotSelectHighlighter, moveButton;
    public Image itemSprite;
    public Text amountText;

    public void Empty_Slot()
    {
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

    public void Select_Slot()
    {
        system.DeSelect_All_Slots();
        slotSelected = true;
        slotSelectHighlighter.SetActive(true);
    }
    public void DeSelect_Slot()
    {
        slotSelected = false;
        slotSelectHighlighter.SetActive(false);
        moveButton.SetActive(false);
    }

    public void Click_Slot()
    {
        if (!slotSelected)
        {
            Select_Slot();

            if (hasItem)
            {
                moveButton.SetActive(true);
            }
        }
        else if (slotSelected)
        {
            DeSelect_Slot();
        }
    }
    public void Move_Slot()
    {
        system.Move_Slot_to_StaticSlotsSystem(currentItem, itemAmount);
        Empty_Slot();
    }
}
