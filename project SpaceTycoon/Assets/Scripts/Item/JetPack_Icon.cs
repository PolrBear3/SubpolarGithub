using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetPack_Icon : MonoBehaviour
{
    // add box collider and rigidbody to icon
    
    private void Awake()
    {
        controller = GetComponent<Inventory_MainController>();
    }

    public GameObject jetPack_icon;
    public GameObject jetPack;
    private bool inInventory = false;

    Inventory_MainController controller;

    public void Move_to_Inventory()
    {
        if (inInventory == false)
        {
            inInventory = true;
            // bc.enabled = false;
            jetPack.SetActive(true);
            controller.Move_to_Inventory(jetPack_icon);
            Destroy(gameObject);
        }
    }

    public void Move_to_Bag()
    {
        if (inInventory == true)
        {
            inInventory = false;
            // bc.enabled = false;
            jetPack.SetActive(false);
            controller.Move_to_Bag(jetPack_icon);
            Destroy(gameObject);
        }
    }
}
