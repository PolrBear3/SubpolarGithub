using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bag_Slot : MonoBehaviour
{
    private void Awake()
    {
        image = GetComponent<Image>();
        controller = GameObject.Find("Inventory Gadget").GetComponent<Inventory_MainController>();
    }

    private void Start()
    {
        slotEmpty = true;
    }

    private void Update()
    {
        Slot_DesignType_Change();
    }

    Image image;
    Inventory_MainController controller;

    [HideInInspector]
    public bool slotEmpty;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item_Icon"))
        {
            slotEmpty = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Item_Icon"))
        {
            slotEmpty = true;
        }
    }

    // slot design change according to bag level
    public Sprite level1, level2;
    void Slot_DesignType_Change()
    {
        if (controller.bagLevel1.activeSelf)
        {
            this.image.sprite = level1;
        }
        else if (controller.bagLevel2.activeSelf)
        {
            this.image.sprite = level2;
        }
    }
}
