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
    public GameObject fireBallhandHeld2;
    public GameObject fireBallslot1;

    public GameObject iceBallhandHeld;
    public GameObject iceBallhandHeld2;
    public GameObject iceBallslot1;

    public GameObject canehandHeld;
    public GameObject canehandHeld2;
    public GameObject caneslot1;

    public GameObject spearhandHeld;
    public GameObject spearhandHeld2;
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
        if (Input.GetKeyDown(KeyCode.E) && playerScanned == true)
        {
            // fireBall
            if (fireBallhandHeld.activeSelf == true && controller.hand1.activeSelf == true)
            {
                fireBallslot1.SetActive(true);
                slot1Empty = false;
                controller.playerObjectController.Default_State();
            }
            else if (fireBallhandHeld2.activeSelf == true && controller.hand2.activeSelf == true)
            {
                fireBallslot1.SetActive(true);
                slot1Empty = false;
                controller.playerSecondObjectController.Default_State();
            }

            // iceBall
            if (iceBallhandHeld.activeSelf == true && controller.hand1.activeSelf == true)
            {
                iceBallslot1.SetActive(true);
                slot1Empty = false;
                controller.playerObjectController.Default_State();
            }
            else if (iceBallhandHeld2.activeSelf == true && controller.hand2.activeSelf == true)
            {
                iceBallslot1.SetActive(true);
                slot1Empty = false;
                controller.playerSecondObjectController.Default_State();
            }

            // cane
            if (canehandHeld.activeSelf == true && controller.hand1.activeSelf == true)
            {
                caneslot1.SetActive(true);
                slot1Empty = false;
                controller.playerObjectController.Default_State();
            }
            else if (canehandHeld2.activeSelf == true && controller.hand2.activeSelf == true)
            {
                caneslot1.SetActive(true);
                slot1Empty = false;
                controller.playerSecondObjectController.Default_State();
            }

            // spear
            if (spearhandHeld.activeSelf == true && controller.hand1.activeSelf == true)
            {
                spearslot1.SetActive(true);
                slot1Empty = false;
                controller.playerObjectController.Default_State();
            }
            else if (spearhandHeld2.activeSelf == true && controller.hand2.activeSelf == true)
            {
                spearslot1.SetActive(true);
                slot1Empty = false;
                controller.playerSecondObjectController.Default_State();
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
