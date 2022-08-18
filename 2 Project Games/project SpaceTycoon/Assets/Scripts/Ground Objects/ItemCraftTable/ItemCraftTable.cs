using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemCraftTable : MonoBehaviour
{
    SpaceTycoon_Main_GameController controller;
    public Guest_System guestSystem;
    public Guest_System craftSlotSystem;

    Animator anim;

    bool playerDetection;
    public Object_ScrObj objectInfo;
    public Icon icon;
    public GameObject[] panels;

    public GameObject[] itemTypeLists;
    [HideInInspector]
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
        currentlyOpenedItemSprite.sprite = itemInfo.UIitemIcon;
        
        guestSystem.Connect_to_HostSystem();

        // open inventory gadget
        var inventoryGadget = guestSystem.hostSystem.inventoryMenu;

        if (!inventoryGadget.inventoryMenuGameObject.activeSelf)
        {
            inventoryGadget.Open_Inventory_Menu();
        }
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

    // craft item functions
    public void Craft_Item()
    {
        var openedItemIngredients = currentlyOpenedItem.ingredients;

        if (guestSystem.checkSystem.Required_Ingredients_Check(currentlyOpenedItem))
        {
            for (int i = 0; i < currentlyOpenedItem.ingredients.Length; i++)
            {
                guestSystem.Use_Items(openedItemIngredients[i].itemInfo, openedItemIngredients[i].amount);
            }
            
            craftSlotSystem.Craft_Item(true, 2, currentlyOpenedItem, 1, currentlyOpenedItem.itemMaxDurability);
        }
        else
        {
            Debug.Log("Not enough ingredients!");
        }
    }
    public void Dismantle()
    {
        controller.Object_Dismantle(objectInfo, 1, icon, gameObject);
    }
}
