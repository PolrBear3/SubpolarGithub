using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapePod_CraftTable : SpaceTycoon_Main_GameController
{
    Animator anim;
    
    public snapPoint[] groundPoints;
    public snapPoint[] wallPoints;
    public GameObject[] objectOptions;
    
    [HideInInspector]
    public bool playerDetection;
    
    public GameObject mainPanel;
    public Icon icon;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        Icon_Popup_UpdateCheck(playerDetection, icon.gameObject);
        Automatic_TurnOff_MainPanel();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("player_hand"))
        {
            playerDetection = true;
            icon.Set_Icon_Position();
            anim.SetBool("onMenu", true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("player_hand"))
        {
            playerDetection = false;
            icon.Set_Icon_to_Default_Position();
            anim.SetBool("onMenu", false);
        }
    }

    void Automatic_TurnOff_MainPanel()
    {
        Automatic_TurnOff_ObjectPanel(playerDetection, mainPanel, objectOptions);
    }
    public void Open_Option()
    {
        TurnOn_Single_Options_inObjectPanel(objectOptions[0]);
    }
    public void Exit_Menu()
    {
        Manual_TurnOff_ObjectPanel(mainPanel, objectOptions);
        TurnOff_All_Options_inObjectPanel(objectOptions);
    }
    public void Exit_Option()
    {
        TurnOff_All_Options_inObjectPanel(objectOptions);
    }
}
