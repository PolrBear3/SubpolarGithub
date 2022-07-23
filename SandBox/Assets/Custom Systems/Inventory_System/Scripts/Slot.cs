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
    private bool Move_Check()
    {
        if (Slots_Controller.saveSlot1 != null)
        {
            return true;
        }
        else return false;
    }

    public void Select_Slot()
    {
        slotSelected = true;
        slotSelectHighlighter.SetActive(true);
    }
    public void DeSelect_Slot()
    {
        slotSelected = false;
        slotSelectHighlighter.SetActive(false);
    }

    //
    public void Click_Slot()
    {
        if (Move_Check())
        {
            Empty_Slot();
            Assign_Slot(Slots_Controller.saveSlot1.currentItem, Slots_Controller.saveSlot1.slotAmount);
            Slots_Controller.Clear_SaveSlot1();
        }
        else
        {
            if (!slotSelected)
            {
                system.DeSelect_All_Slots();
                Select_Slot();
                Slots_Controller.Assign_SaveSlot(this);
            }
            else if (slotSelected)
            {
                DeSelect_Slot();
                Slots_Controller.Clear_SaveSlot1();
            }
        }
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
