using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetPack_Icon_Bag : MonoBehaviour
{
    private void Awake()
    {
        bc = GetComponent<BoxCollider2D>();
        controller = GameObject.Find("Inventory").GetComponent<Inventory_MainController>();
    }

    BoxCollider2D bc;
    Inventory_MainController controller;
    
    public GameObject jetPack_icon_Inventory;

    public void Move_to_Inventory()
    {
        // instantiate inventory icon to backSlot
        Instantiate_to_Inventory();
        // enable jetpack on player
        controller.jetPack.SetActive(true);
        // box collider off
        bc.enabled = false;
        // destroy this gameobject
        Destroy(gameObject);
    }

    void Instantiate_to_Inventory()
    {
        if (controller.inventory.backSlotEmpty == true)
        {
            Instantiate(jetPack_icon_Inventory, controller.backSlot);
        }
    }
}
