using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapePod_CraftTable : SpaceTycoon_Main_GameController
{
    Animator anim;

    // gameobject ground and wall snappoints
    public GameObject[] groundSnapPoints;
    public snapPoint[] groundPoints;
    public GameObject[] groundSnapPointButtons;
    public SnapPoint_Button[] groundButtons;

    public GameObject[] wallSnapPoints;
    public snapPoint[] wallPoints;
    public GameObject[] wallSnapPointButtons;
    public SnapPoint_Button[] wallButtons;

    public GameObject[] objectOptionMenus;

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
        Move_CraftTable();
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

    // Basic Menu Functions
    void Automatic_TurnOff_MainPanel()
    {
        Automatic_TurnOff_ObjectPanel(playerDetection, mainPanel, objectOptionMenus);
    }
    public void Open_Option()
    {
        TurnOn_Single_Options_inObjectPanel(objectOptionMenus[0]);
    }
    public void Exit_Menu()
    {
        Manual_TurnOff_ObjectPanel(mainPanel, objectOptionMenus);
        TurnOff_All_Options_inObjectPanel(objectOptionMenus);
    }
    public void Exit_Option()
    {
        TurnOff_All_Options_inObjectPanel(objectOptionMenus);
    }

    // Options and Craft Functions
    void SnapPoint_Availability_Update()
    {
        for (int i = 0; i < groundPoints.Length; i++)
        {
            groundPoints[i].Object_Placed_Check();

            if (groundPoints[i].objectPlaced)
            {
                groundSnapPointButtons[i].SetActive(false);
            }
            else
            {
                groundSnapPointButtons[i].SetActive(true);
            }
        }
    }
    public void Move_CraftTable()
    {
        for (int i = 0; i < groundButtons.Length; i++)
        {
            if (groundButtons[i].buttonPressed)
            {
                gameObject.transform.position = groundPoints[i].gameObject.transform.position;
                gameObject.transform.parent = groundPoints[i].gameObject.transform;
                groundButtons[i].Set_Backto_UnPressed();
                break;
            }
        }
        SnapPoint_Availability_Update();
    }
}
