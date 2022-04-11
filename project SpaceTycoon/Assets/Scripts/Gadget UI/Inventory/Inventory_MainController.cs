using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_MainController : MonoBehaviour
{
    // inventory slots
    public Item_Slot currentS, backS, throwable1S, throwable2S;
    public Transform currentT, backT, throwable1T, throwable2T;
    
    // bag slots
    public Item_Slot bag1S, bag2S, bag3S, bag4S;
    public Transform bag1T, bag2T, bag3T, bag4T;

    // player items
    public GameObject jetPack;

    // inventory menu
    public GameObject inventoryMenu;

    // icon instantiate functions
    public void Instantiate_Inventory_Icon(string itemType, GameObject inventoryIcon, GameObject item, BoxCollider2D bc, GameObject thisIcon)
    {
        if (currentS.slotEmpty && itemType == "current")
        {
            Instantiate(inventoryIcon, currentT);
            item.SetActive(true);
            bc.enabled = false;
            Destroy(thisIcon);
        }
        else if (backS.slotEmpty && itemType == "back")
        {
            Instantiate(inventoryIcon, backT);
            item.SetActive(true);
            bc.enabled = false;
            Destroy(thisIcon);
        }
        else if (throwable1S.slotEmpty && itemType == "throwable1")
        {
            Instantiate(inventoryIcon, throwable1T);
            item.SetActive(true);
            bc.enabled = false;
            Destroy(thisIcon);
        }
        else if (throwable2S.slotEmpty && itemType == "throwable2")
        {
            Instantiate(inventoryIcon, throwable2T);
            item.SetActive(true);
            bc.enabled = false;
            Destroy(thisIcon);
        }
    }
    public void Instantiate_Bag_Icon(GameObject bagIcon, GameObject item, BoxCollider2D bc, GameObject thisIcon)
    {
        if (bag1S.slotEmpty)
        {
            Instantiate(bagIcon, bag1T);
            item.SetActive(false);
            bc.enabled = false;
            Destroy(thisIcon);
        }
        else if (bag2S.slotEmpty)
        {
            Instantiate(bagIcon, bag2T);
            item.SetActive(false);
            bc.enabled = false;
            Destroy(thisIcon);
        }
        else if (bag3S.slotEmpty)
        {
            Instantiate(bagIcon, bag3T);
            item.SetActive(false);
            bc.enabled = false;
            Destroy(thisIcon);
        }
        else if (bag4S.slotEmpty)
        {
            Instantiate(bagIcon, bag4T);
            item.SetActive(false);
            bc.enabled = false;
            Destroy(thisIcon);
        }
    }
}
