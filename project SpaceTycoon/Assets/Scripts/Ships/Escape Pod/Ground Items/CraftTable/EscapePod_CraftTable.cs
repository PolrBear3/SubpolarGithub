using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapePod_CraftTable : SpaceTycoon_Main_GameController
{
    Animator anim;

    // gameobject ground and wall snappoints
    public snapPoint[] groundSnapPoints;
    public GameObject[] groundSnapPointButtons;
    public SnapPoint_Button[] groundButtons;

    public snapPoint[] wallSnapPoints;
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
        GroundSnapPoints_Sprite_Off();
        WallSnapPoints_Sprite_Off();
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

            GroundSnapPoints_Sprite_Off();
            WallSnapPoints_Sprite_Off();
        }
    }

    // Basic Menu Functions
    void Automatic_TurnOff_MainPanel()
    {
        Automatic_TurnOff_ObjectPanel(playerDetection, mainPanel, objectOptionMenus);
    }
    public void Exit_Menu()
    {
        Manual_TurnOff_ObjectPanel(mainPanel, objectOptionMenus);
        TurnOff_All_Options_inObjectPanel(objectOptionMenus);
    }
    public void Open_Option()
    {
        TurnOn_Single_Options_inObjectPanel(objectOptionMenus[0]);
        GroundSnapPoints_Sprite_On();
        SnapPoint_Availability_Update();
    }
    public void Exit_Option()
    {
        TurnOff_All_Options_inObjectPanel(objectOptionMenus);
        GroundSnapPoints_Sprite_Off();
        WallSnapPoints_Sprite_Off();
    }

    // Options SnapPoint Sprite ON and OFF
    public void GroundSnapPoints_Sprite_On()
    {
        for (int i = 0; i < groundSnapPoints.Length; i++)
        {
            groundSnapPoints[i].Sprite_On();
        }
    }
    public void GroundSnapPoints_Sprite_Off()
    {
        for (int i = 0; i < groundSnapPoints.Length; i++)
        {
            groundSnapPoints[i].Sprite_Off();
        }
    }

    public void WallSnapPoints_Sprite_On()
    {
        for (int i = 0; i < wallSnapPoints.Length; i++)
        {
            wallSnapPoints[i].Sprite_Off();
        }
    }
    public void WallSnapPoints_Sprite_Off()
    {
        for (int i = 0; i < wallSnapPoints.Length; i++)
        {
            wallSnapPoints[i].Sprite_Off();
        }
    }

    // Options and Craft Functions
    void SnapPoint_Availability_Update()
    {
        for (int i = 0; i < groundSnapPoints.Length; i++)
        {
            groundSnapPoints[i].Object_Placed_Check();

            if (groundSnapPoints[i].objectPlaced)
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
                gameObject.transform.position = groundSnapPoints[i].gameObject.transform.position;
                gameObject.transform.parent = groundSnapPoints[i].gameObject.transform;
                groundButtons[i].Set_Backto_UnPressed();
                break;
            }
        }
        SnapPoint_Availability_Update();
    }
}
