using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemCraftTable : MonoBehaviour
{
    SpaceTycoon_Main_GameController controller;
    private Guest_System guestSystem;

    Animator anim;

    bool playerDetection;
    public Object_ScrObj objectInfo;
    public Icon icon;
    public GameObject[] panels;

    public GameObject[] itemTypeLists;
    public Item_Info currentlyOpenedItem;
    public Item_Info jetPack;
    public Image currentlyOpenedItemSprite;

    private void Awake()
    {
        controller = GameObject.FindGameObjectWithTag("SpaceTycoon Main GameController").GetComponent<SpaceTycoon_Main_GameController>();
        guestSystem = GetComponent<Guest_System>();
        anim = GetComponent<Animator>();
    }
    private void Start()
    {
        Reset_All_ItemLists();
        Open_Hand_ItemList();
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

            guestSystem.Disconnect_HostSystem();
        }
    }

    // UI Basic Functions
    public void Open_MainPanel()
    {
        controller.Icon_Pressed(panels[0]);
    }
    public void Open_Item_OptionPanel(Item_Info itemInfo)
    {
        controller.TurnOn_Single_Options_inObjectPanel(panels[1]);
        currentlyOpenedItem = itemInfo;
        
        guestSystem.Connect_to_HostSystem();
        // connect item ingredients
    }
    public void Exit_Object()
    {
        controller.Manual_TurnOff_ObjectPanel(panels[0]);
        controller.TurnOff_Single_Options_inObjectPanel(panels[1]);
        guestSystem.Disconnect_HostSystem();
    }
    public void Exit_OpitonPanel()
    {
        controller.TurnOff_Single_Options_inObjectPanel(panels[1]);
        guestSystem.Disconnect_HostSystem();
    }

    // Item Type Button Page Controller
    private void Reset_All_ItemLists()
    {
        for (int i = 0; i < itemTypeLists.Length; i++)
        {
            itemTypeLists[i].SetActive(false);
        }
    }

    public void Open_Hand_ItemList()
    {
        Reset_All_ItemLists();
        itemTypeLists[0].SetActive(true);
    }
    public void Open_Back_ItemList()
    {
        Reset_All_ItemLists();
        itemTypeLists[1].SetActive(true);
    }
    public void Open_Throwable_ItemList()
    {
        Reset_All_ItemLists();
        itemTypeLists[2].SetActive(true);
    }

    // craft item
    public void Craft_Item()
    {
        guestSystem.Craft_Item(true, 2, currentlyOpenedItem, 1, currentlyOpenedItem.itemMaxDurability);
    }
}
