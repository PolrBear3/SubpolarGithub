using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_Slot : MonoBehaviour
{
    private void Start()
    {
        slotEmpty = true;
    }

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
}
