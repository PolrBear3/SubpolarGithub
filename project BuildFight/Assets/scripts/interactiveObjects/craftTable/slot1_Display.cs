using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slot1_Display : MonoBehaviour
{
    public CraftTable_Controller controller;
    
    bool playerScanned;
    [HideInInspector]
    public bool slot1Empty = true;

    /// ///////////////////////////////////////////////////////////////////////
    public GameObject fireBallhandHeld;
    public GameObject fireBallslot1;

    public GameObject iceBallhandHeld;
    public GameObject iceBallslot1;

    public GameObject canehandHeld;
    public GameObject caneslot1;

    public GameObject spearhandHeld;
    public GameObject spearslot1;

    void Update()
    {
        Display_Object();
        Clear_Slot();
    }

    void Clear_Slot()
    {
        if (slot1Empty == true)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    void Display_Object()
    {
        if (Input.GetKeyDown(KeyCode.E) && object_controller.hasItem == true && playerScanned == true)
        {
            if (fireBallhandHeld.activeSelf == true)
            {
                fireBallhandHeld.SetActive(false);
                fireBallslot1.SetActive(true);
                slot1Empty = false;
                object_controller.hasItem = false;
            }

            if (iceBallhandHeld.activeSelf == true)
            {
                iceBallhandHeld.SetActive(false);
                iceBallslot1.SetActive(true);
                slot1Empty = false;
                object_controller.hasItem = false;
            }

            if (canehandHeld.activeSelf == true)
            {
                canehandHeld.SetActive(false);
                caneslot1.SetActive(true);
                slot1Empty = false;
                object_controller.hasItem = false;
            }

            if (spearhandHeld.activeSelf == true)
            {
                spearhandHeld.SetActive(false);
                spearslot1.SetActive(true);
                slot1Empty = false;
                object_controller.hasItem = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("player"))
        {
            playerScanned = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("player"))
        {
            playerScanned = false;
        }
    }
}
