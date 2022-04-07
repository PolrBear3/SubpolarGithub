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
        controller.Instantiate_Inventory_Icon("back", jetPack_icon_Inventory, controller.jetPack, bc, gameObject);
    }
}
