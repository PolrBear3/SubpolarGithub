using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapePod_ChairBed_Panel : SpaceTycoon_Main_GameController
{
    public GameObject EscapePod_ChairBed_gameObject;
    public EscapePod_ChairBed_MainController controller;

    private void Start()
    {
        controller.rotateRightButton.SetActive(false);
    }

    private void Update()
    {
        Icon_Popup();
        Automatic_Off();
        Check_Mode_for_Player_Action();
    }

    // icon
    void Icon_Popup() 
    {
        // is on 
        if (controller.playerDetection == true && isPanelMenuOn == false)
        {
            controller.Icon.SetActive(true);
        }
        // is off
        if (controller.playerDetection == false || isPanelMenuOn == true || 
          Player_State.player_isSitting == true || Player_State.player_isSleeping == true)
        {
            controller.Icon.SetActive(false);
        }
    }
    public void Icon_Pressed()
    {
        controller.mainPanel.SetActive(true);
        isPanelMenuOn = true;
    }

    // main panel
    public void Manual_Off()
    {
        controller.mainPanel.SetActive(false);
        isPanelMenuOn = false;
    }
    void Automatic_Off()
    {
        if (controller.playerDetection == false && controller.mainPanel.activeSelf == true)
        {
            controller.mainPanel.SetActive(false);
            isPanelMenuOn = false;
        }
    }
    
    // functions
    private bool facingLeft = false;
    public void Rotate_Right()
    {
        facingLeft = !facingLeft;
        EscapePod_ChairBed_gameObject.transform.Rotate(0f, 180f, 0f);
        controller.rotateRightButton.SetActive(false);
        controller.roateLeftButton.SetActive(true);
    }
    public void Rotate_Left()
    {
        facingLeft = !facingLeft;
        EscapePod_ChairBed_gameObject.transform.Rotate(0f, 180f, 0f);
        controller.roateLeftButton.SetActive(false);
        controller.rotateRightButton.SetActive(true);
    }

    public void Dismantle()
    {
        // chairBed_storage += 1;
        controller.iconBoxCollider.SetActive(false);
        Destroy(EscapePod_ChairBed_gameObject);
    }

    public void Change_to_Bed()
    {
        controller.sr.sprite = controller.bed;
        controller.chairMode = false;
    }

    public void Change_to_Chair()
    {
        controller.sr.sprite = controller.chair;
        controller.chairMode = true;
    }

    void Check_Mode_for_Player_Action()
    {
        if (controller.chairMode == true)
        {
            controller.sitButton.SetActive(true);
            controller.sleepButton.SetActive(false);
        }
        else
        {
            controller.sitButton.SetActive(false);
            controller.sleepButton.SetActive(true);
        }
    }

    public void Sit()
    {
        // set player's position to this chairbed's sit position
        controller.player.transform.position = new Vector2(controller.sitPosition.transform.position.x, controller.sitPosition.transform.position.y);

        // set player's animation to sit
        Player_State.player_isSitting = true;

        // flip the player to chairBed facing position
        if (!facingLeft && Player_State.player_isFacing_Left)
        {
            controller.player.transform.Rotate(0f, 180f, 0f);
            Player_State.player_isFacing_Left = !Player_State.player_isFacing_Left;
        }

        // main panel automatic off
        controller.mainPanel.SetActive(false);
        isPanelMenuOn = false;

        // player sitting = true, gains sleep energy
    }
    public void Sleep()
    {
        // set player's position to this chairbed's sleep position
        controller.player.transform.position = new Vector2(controller.sleepPosition.transform.position.x, controller.sleepPosition.transform.position.y);

        // set player's animation to sleep
        Player_State.player_isSleeping = true;

        // flip the player to chairBed facing position
        if (!facingLeft && Player_State.player_isFacing_Left)
        {
            controller.player.transform.Rotate(0f, 180f, 0f);
            Player_State.player_isFacing_Left = !Player_State.player_isFacing_Left;
        }

        // main panel automatic off
        controller.mainPanel.SetActive(false);
        isPanelMenuOn = false;

        // player sleeping = true, gains sleep energy
    }
}
