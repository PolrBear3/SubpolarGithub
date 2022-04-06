using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetPack_Icon : MonoBehaviour
{
    private void Awake()
    {
        controller = GameObject.Find("Inventory").GetComponent<Inventory_MainController>();
        bc = GetComponent<BoxCollider2D>();
    }

    BoxCollider2D bc;
    Inventory_MainController controller;
    
    public GameObject jetPack_icon;
    public GameObject jetPack;
    private bool inInventory = false;


    public void Move_to_Inventory()
    {
        if (inInventory == false)
        {
            inInventory = true;
            bc.enabled = false;
            jetPack.SetActive(true);
            controller.Move_to_Inventory(jetPack_icon, "backType");
            Destroy(gameObject);
        }
    }

    public void Move_to_Bag()
    {
        if (inInventory == true)
        {
            inInventory = false;
            bc.enabled = false;
            jetPack.SetActive(false);
            controller.Move_to_Bag(jetPack_icon);
            Destroy(gameObject);
        }
    }
}
