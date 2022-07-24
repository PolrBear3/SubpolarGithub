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

    public GameObject slotSelectHighlighter;
    public Image itemSprite;
    public Text amountText;

    public void Empty_Slot()
    {
        currentItem = null;
        itemSprite.sprite = null;
        itemSprite.color = Color.clear;
        amountText.text = "";
    }
    public void Assign_Slot(Item_Info itemInfo, int itemAmount)
    {
        currentItem = itemInfo;
        this.itemAmount = itemAmount;
        itemSprite.sprite = itemInfo.itemSprite;
        itemSprite.color = Color.white;
        amountText.text = itemAmount.ToString();
    }

    public void Move_Slot()
    {
        system.Move_Slot_to_SlotsSystem(currentItem, itemAmount);
    }
}
