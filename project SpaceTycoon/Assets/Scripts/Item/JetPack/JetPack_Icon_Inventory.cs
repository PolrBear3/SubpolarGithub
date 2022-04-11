using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetPack_Icon_Inventory : MonoBehaviour
{
    private void Awake()
    {
        bc = GetComponent<BoxCollider2D>();
        controller = GameObject.Find("Inventory Gadget Option").GetComponent<Inventory_MainController>();
    }

    BoxCollider2D bc;
    Inventory_MainController controller;

    public GameObject jetPack_icon_Bag;

    public void Move_to_Bag()
    {
        controller.Instantiate_Bag_Icon(jetPack_icon_Bag, controller.jetPack, bc, gameObject);
    }
}
