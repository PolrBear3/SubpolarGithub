using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour
{
    public bool slotSelected = false;
    public bool hasItem = false;

    public Slots_System system;
    public Item_Info currentItem;
    public int itemAmount;

    public GameObject slotSelectHighlighter;
    public Image itemSprite;
    public Text amountText;

    //
    private void Slot_Select()
    {
        system.DeSelect_All_Slots();
        slotSelected = true;
        slotSelectHighlighter.SetActive(true);
        system.Save_Slot_DataBase(this);
    }
    public void Slot_DeSelect()
    {
        slotSelected = false;
        slotSelectHighlighter.SetActive(false);
    }
    public void Click_Slot()
    {
        if (!slotSelected)
        {
            Slot_Select();
        }
        else if (slotSelected)
        {
            Slot_DeSelect();
        }
    }

    //
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
}
