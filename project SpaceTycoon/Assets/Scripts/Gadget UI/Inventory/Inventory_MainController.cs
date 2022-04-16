using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_MainController : MonoBehaviour
{
    public GameObject jetPackBagIcon;

    private void Start()
    {
        // jetpack made to bag slot example
        Instantiate(jetPackBagIcon, bag1T);
    }

    public Animator anim;

    // inventory slots
    public Inventory_Slot currentS, backS, throwable1S, throwable2S;
    public Transform currentT, backT, throwable1T, throwable2T;

    // bag slots
    public List<GameObject> Level2BagSlots;

    public Bag_Slot bag1S, bag2S, bag3S, bag4S, // level 1
                    bag5S, bag6S, bag7S, bag8S; // level 2
    
    public Transform bag1T, bag2T, bag3T, bag4T, // level 1
                     bag5T, bag6T, bag7T, bag8T; // level 2

    // bag sprite
    public GameObject bagLevel1, bagLevel2;
    public void Reset_Bag_Level()
    {
        bagLevel1.SetActive(false);
        bagLevel2.SetActive(false);
    }

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
        else if (bag5S.slotEmpty)
        {
            Instantiate(bagIcon, bag5T);
            item.SetActive(false);
            bc.enabled = false;
            Destroy(thisIcon);
        }
        else if (bag6S.slotEmpty)
        {
            Instantiate(bagIcon, bag6T);
            item.SetActive(false);
            bc.enabled = false;
            Destroy(thisIcon);
        }
        else if (bag7S.slotEmpty)
        {
            Instantiate(bagIcon, bag7T);
            item.SetActive(false);
            bc.enabled = false;
            Destroy(thisIcon);
        }
        else if (bag8S.slotEmpty)
        {
            Instantiate(bagIcon, bag8T);
            item.SetActive(false);
            bc.enabled = false;
            Destroy(thisIcon);
        }
    }
}
