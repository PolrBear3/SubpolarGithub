using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct OutfitUnlock
{
    public Albert_Outfits outfitInfo;
    public bool unlocked;
    public GameObject lockedImage;
    public GameObject unlockedImage;
}

public class Closet : MonoBehaviour
{
    SpaceTycoon_Main_GameController controller;
    Player_MainController player;
    Animator anim;

    public OutfitUnlock[] unlockCheckOutfits;

    bool playerDetection;
    public Object_ScrObj objectInfo;
    public Icon icon;
    public GameObject mainPanel, optionPanel;
    public int openedOutfitID;
    public Image previewImage;

    private void Awake()
    {
        controller = GameObject.FindGameObjectWithTag("SpaceTycoon Main GameController").GetComponent<SpaceTycoon_Main_GameController>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_MainController>();
        anim = GetComponent<Animator>();
    }
    private void Start()
    {
        Unlock_Check_for_Image();
    }
    private void Update()
    {
        controller.Icon_Popup_UpdateCheck(playerDetection, icon.gameObject, 2);
        controller.Automatic_TurnOff_ObjectPanel(playerDetection, mainPanel);
        controller.Automatic_TurnOff_Single_Options_inObjectPanel(playerDetection, optionPanel);
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

    // basic functions
    public void Open_MainPanel()
    {
        controller.Icon_Pressed(mainPanel);
    }
    public void Exit_MainPanel()
    {
        controller.Manual_TurnOff_ObjectPanel(mainPanel);
    }
    public void Open_Option(Albert_Outfits outfitInfo)
    {
        for (int i = 0; i < unlockCheckOutfits.Length; i++)
        {
            Exit_Option();
            
            if (outfitInfo.outfitID == unlockCheckOutfits[i].outfitInfo.outfitID)
            {
                openedOutfitID = unlockCheckOutfits[i].outfitInfo.outfitID;
                previewImage.sprite = outfitInfo.outfitSprite;
                
                if (!unlockCheckOutfits[openedOutfitID].unlocked)
                {
                    controller.TurnOn_Single_Options_inObjectPanel(optionPanel);
                }
                else if (unlockCheckOutfits[openedOutfitID].unlocked)
                {
                    player.playerOutfit.Update_Player_Outfit(unlockCheckOutfits[openedOutfitID].outfitInfo);
                }
                break;
            }
        }
    }
    public void Exit_Option()
    {
        controller.TurnOff_Single_Options_inObjectPanel(optionPanel);
    }
    public void Dismantle()
    {
        controller.Object_Dismantle(objectInfo, 1, icon, gameObject);
    }

    // closet function
    public void Outfit_Craft()
    {
        unlockCheckOutfits[openedOutfitID].unlocked = true;
        Unlock_Check_for_Image();
        Exit_Option();
    }
    public void Unlock_Check_for_Image()
    {
        for (int i = 0; i < unlockCheckOutfits.Length; i++)
        {
            if (unlockCheckOutfits[i].unlocked)
            {
                unlockCheckOutfits[i].unlockedImage.SetActive(true);
                unlockCheckOutfits[i].lockedImage.SetActive(false);

            }
            else if (!unlockCheckOutfits[i].unlocked)
            {
                unlockCheckOutfits[i].unlockedImage.SetActive(false);
                unlockCheckOutfits[i].lockedImage.SetActive(true);
            }
        }
    }
}
