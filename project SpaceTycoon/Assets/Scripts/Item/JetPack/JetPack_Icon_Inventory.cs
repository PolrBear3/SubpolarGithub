using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetPack_Icon_Inventory : MonoBehaviour
{
    private void Awake()
    {
        bc = GetComponent<BoxCollider2D>();
        controller = GameObject.Find("Inventory").GetComponent<Inventory_MainController>();
    }

    BoxCollider2D bc;
    Inventory_MainController controller;

    public GameObject jetPack_icon_Bag;

    public void Move_to_Bag()
    {
        Instantiate_to_Bag();
        // enable jetpack on player
        controller.jetPack.SetActive(false);
        // box collider off
        bc.enabled = false;
        // destroy this gameobject
        Destroy(gameObject);
    }

    public void Instantiate_to_Bag()
    {
        if (controller.bag.slot1Empty == true)
        {
            Instantiate(jetPack_icon_Bag, controller.bagSlot1);
        }
        else if (controller.bag.slot2Empty == true)
        {
            Instantiate(jetPack_icon_Bag, controller.bagSlot2);
        }
        else if (controller.bag.slot3Empty == true)
        {
            Instantiate(jetPack_icon_Bag, controller.bagSlot3);
        }
        else if (controller.bag.slot4Empty == true)
        {
            Instantiate(jetPack_icon_Bag, controller.bagSlot4);
        }
    }
}
