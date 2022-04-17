using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemIcon_Detector : MonoBehaviour
{
    [HideInInspector]
    public bool itemIconDetection = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("fuelItem_Icon"))
        {
            itemIconDetection = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("fuelItem_Icon"))
        {
            itemIconDetection = false;
        }
    }
}
