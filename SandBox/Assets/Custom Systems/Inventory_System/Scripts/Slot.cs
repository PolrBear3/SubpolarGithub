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
    public int slotAmount;

    public GameObject slotSelectHighlighter;
    public Image itemSprite;
    public Text amountText;

    //


    //
    public void Select_Slot()
    {
        if (!slotSelected)
        {
            system.DeSelect_All_Slots();
            slotSelected = true;
            slotSelectHighlighter.SetActive(true);
        }
        else if (slotSelected)
        {
            DeSelect_Slot();
        }
    }
    public void DeSelect_Slot()
    {
        slotSelected = false;
        slotSelectHighlighter.SetActive(false);
    }

    //
    public void Empty_Slot()
    {
        itemSprite.sprite = null;
        itemSprite.color = Color.clear;
        amountText.text = "";
    }
    public void Assign_Slot(Item_Info itemInfo, int itemAmount)
    {
        currentItem = itemInfo;
        itemSprite.sprite = itemInfo.itemSprite;
        itemSprite.color = Color.white;
        amountText.text = itemAmount.ToString();
    }
}
