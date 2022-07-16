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
    public GameObject[] panels;

    private void Awake()
    {
        controller = GameObject.FindGameObjectWithTag("SpaceTycoon Main GameController").GetComponent<SpaceTycoon_Main_GameController>();
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        controller.Icon_Popup_UpdateCheck(playerDetection, icon.gameObject);
        controller.Automatic_TurnOff_ObjectPanel(playerDetection, panels[0]);
        controller.Automatic_TurnOff_Single_Options_inObjectPanel(playerDetection, panels[1]);
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
            playerDetection = false;
            icon.Set_Icon_to_Default_Position();
            anim.SetBool("playerDetected", false);
        }
    }

    // UI Basic Functions
    public void Open_MainPanel()
    {
        controller.Icon_Pressed(panels[0]);
    }
    public void Exit_Object()
    {
        controller.Manual_TurnOff_ObjectPanel(panels[0]);
        controller.TurnOff_Single_Options_inObjectPanel(panels[1]);
    }
    public void Exit_OpitonPanel()
    {
        controller.TurnOff_Single_Options_inObjectPanel(panels[1]);
    }
}
