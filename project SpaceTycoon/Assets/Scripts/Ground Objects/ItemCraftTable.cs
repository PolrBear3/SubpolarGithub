using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCraftTable : MonoBehaviour
{
    SpaceTycoon_Main_GameController controller;
    // player inventory holder
    Animator anim;

    bool playerDetection;
    public Object_ScrObj objectInfo;
    public Icon icon;

    private void Awake()
    {
        controller = GameObject.FindGameObjectWithTag("SpaceTycoon Main GameController").GetComponent<SpaceTycoon_Main_GameController>();
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("player_hand"))
        {
            playerDetection = true;
            icon.Set_Icon_Position();
            anim.SetBool("playerDetected", true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("player_hand"))
        {
            playerDetection = true;
            icon.Set_Icon_Position();
            anim.SetBool("playerDetected", false);
        }
    }
}
