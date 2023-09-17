using UnityEngine;
using UnityEngine.UI;

public class EscapePod_CraftTable : MonoBehaviour
{
    SpaceTycoon_Main_GameController controller;
    public Guest_System guestSystem;

    Animator anim;
    [HideInInspector]
    public bool playerDetection;

    // gameobject ground and wall snappoints
    public snapPoint[] groundSnapPoints;
    public snapPoint_Buttons[] groundSnapPointButtons;

    public snapPoint[] wallSnapPoints;
    public snapPoint_Buttons[] wallSnapPointButtons;

    [HideInInspector]
    public Object_ScrObj currentOpenedObject;
    public Button_Detector[] objectCraftOptionButtons;

    public GameObject mainPanel, optionPanel;
    public Image currentSelectedObjImage;
    public Text currentSelectedObjDescription, currentSelectedObjStorageAmount;
    public Sprite deafultObjSprite;
    public string deafultObjDescription;
    public Icon icon;

    private void Awake()
    {
        controller = GameObject.FindGameObjectWithTag("SpaceTycoon Main GameController").GetComponent<SpaceTycoon_Main_GameController>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        SnapPoint_Button_Availability_Update();
        GroundSnapPoints_Off();
        WallSnapPoints_Off();
    }

    private void Update()
    {
        controller.Icon_Popup_UpdateCheck(playerDetection, icon.gameObject, 1);
        controller.Automatic_TurnOff_ObjectPanel(playerDetection, mainPanel);
        controller.Automatic_TurnOff_Single_Options_inObjectPanel(playerDetection, optionPanel);
        Automatic_SnapPoint_Off();
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

            // system disconnect
            guestSystem.Disconnect_HostSystem();

            // exit inventory gadget
            var inventoryGadget = guestSystem.hostSystem.inventoryMenu;

            if (inventoryGadget.inventoryMenuGameObject.activeSelf)
            {
                inventoryGadget.Close_Inventory_Menu();
            }
        }
    }

    // Basic Menu Functions
    public void Open_MainPanel()
    {
        controller.Icon_Pressed(mainPanel);
    }
    public void Exit_Menu()
    {
        controller.Manual_TurnOff_ObjectPanel(mainPanel);
        controller.TurnOff_Single_Options_inObjectPanel(optionPanel);
        GroundSnapPoints_Off();
        WallSnapPoints_Off();

        // system disconnect
        guestSystem.Disconnect_HostSystem();

        // exit inventory gadget
        var inventoryGadget = guestSystem.hostSystem.inventoryMenu;

        if (inventoryGadget.inventoryMenuGameObject.activeSelf)
        {
            inventoryGadget.Close_Inventory_Menu();
        }
    }

    // Options SnapPoint ON and OFF
    private void SnapPoint_Button_Availability_Update()
    {
        // snappoint update
        for (int i = 0; i < groundSnapPoints.Length; i++)
        {
            groundSnapPoints[i].Object_Placed_Check();

            if (groundSnapPoints[i].objectPlaced)
            {
                groundSnapPointButtons[i].SnapPoint_Button_UnAvailable();
            }
            else
            {
                groundSnapPointButtons[i].SnapPoint_Button_Available();
            }
        }
    }

    private void Automatic_SnapPoint_Off()
    {
        // guestSystem.checkSystem.Object_Required_Ingreients_Check()
        if (!playerDetection)
        {
            GroundSnapPoints_Off();
            WallSnapPoints_Off();
        }
    }

    public void GroundSnapPoints_On()
    {
        for (int i = 0; i < groundSnapPoints.Length; i++)
        {
            groundSnapPoints[i].Sprite_On();
            groundSnapPointButtons[i].SnapPoint_Button_Available();
        }
        SnapPoint_Button_Availability_Update();
    }
    public void GroundSnapPoints_Off()
    {
        for (int i = 0; i < groundSnapPoints.Length; i++)
        {
            groundSnapPoints[i].Sprite_Off();
            groundSnapPointButtons[i].SnapPoint_Button_UnAvailable();
        }
    }

    public void WallSnapPoints_On()
    {
        for (int i = 0; i < wallSnapPoints.Length; i++)
        {
            wallSnapPoints[i].Sprite_Off();
            wallSnapPointButtons[i].SnapPoint_Button_Available();
        }
        SnapPoint_Button_Availability_Update();
    }
    public void WallSnapPoints_Off()
    {
        for (int i = 0; i < wallSnapPoints.Length; i++)
        {
            wallSnapPoints[i].Sprite_Off();
            wallSnapPointButtons[i].SnapPoint_Button_UnAvailable();
        }
    }

    // storage system
    private void Storage_Text_Update(Object_ScrObj objectInfo)
    {
        for (int i = 0; i < controller.objectStorages.Length; i++)
        {
            if (objectInfo == controller.objectStorages[i].objectInfo)
            {
                currentSelectedObjStorageAmount.text = controller.objectStorages[i].leftAmount.ToString();
                break;
            }
        }
    }
    private bool Storage_Extra_Amount_Check(Object_ScrObj currentOpenedObject)
    {
        for (int i = 0; i < controller.objectStorages.Length; i++)
        {
            if (currentOpenedObject == controller.objectStorages[i].objectInfo)
            {
                if (controller.objectStorages[i].leftAmount > 0)
                {
                    return true;
                }
            }
        }
        return false;
    }
    private void Dispose_Object_inStorage(Object_ScrObj currentOpenedObject)
    {
        for (int i = 0; i < controller.objectStorages.Length; i++)
        {
            if (currentOpenedObject == controller.objectStorages[i].objectInfo)
            {
                controller.objectStorages[i].leftAmount -= 1;
            }
        }

        Current_Object_Preview(currentOpenedObject);
    }

    // Option Open
    public void Exit_Option()
    {
        controller.TurnOff_Single_Options_inObjectPanel(optionPanel);
        GroundSnapPoints_Off();
        WallSnapPoints_Off();

        // system disconnect
        guestSystem.Disconnect_HostSystem();

        // exit inventory gadget
        var inventoryGadget = guestSystem.hostSystem.inventoryMenu;

        if (inventoryGadget.inventoryMenuGameObject.activeSelf)
        {
            inventoryGadget.Close_Inventory_Menu();
        }
    }
    private void Current_Object_Preview(Object_ScrObj objectInfo)
    {
        currentSelectedObjImage.sprite = objectInfo.objectSprite;
        currentSelectedObjDescription.text = objectInfo.objectDescription;
        Storage_Text_Update(objectInfo);

        if (currentSelectedObjImage.sprite == null)
        {
            currentSelectedObjImage.sprite = deafultObjSprite;
            currentSelectedObjDescription.text = deafultObjDescription;
            currentSelectedObjStorageAmount.text = " ";
        }
    }
    public void Open_Option_for_Object(Object_ScrObj objectInfo) 
    {
        Exit_Option();
        currentOpenedObject = objectInfo;
        SnapPoint_Button_Availability_Update();

        controller.TurnOn_Single_Options_inObjectPanel(optionPanel);

        // system connection
        guestSystem.Connect_to_HostSystem();

        // open inventory gadget
        var inventoryGadget = guestSystem.hostSystem.inventoryMenu;
        if (!inventoryGadget.inventoryMenuGameObject.activeSelf)
        {
            inventoryGadget.Open_Inventory_Menu();
        }

        // in game snappoint on
        if (currentOpenedObject.objectType == ObjectType.ground || currentOpenedObject.gameObjectPrefab == null)
        {
            GroundSnapPoints_On();
        }
        else if (currentOpenedObject.objectType == ObjectType.wall)
        {
            WallSnapPoints_On();
        }

        Current_Object_Preview(currentOpenedObject);
    }

    // Crafting 
    private void Use_Items_inSlots_forCraft(Object_ScrObj objectInfo)
    {
        var openedObjectIngredients = objectInfo.ingredients;
        
        for (int i = 0; i < openedObjectIngredients.Length; i++)
        {
            guestSystem.Use_Items(openedObjectIngredients[i].itemInfo, openedObjectIngredients[i].amount);
        }
    }

    public void Ground_Object_Craft()
    {
        for (int i = 0; i < groundSnapPointButtons.Length; i++)
        {
            // change crafttable position
            if (currentOpenedObject.gameObjectPrefab == null)
            {
                if (groundSnapPointButtons[i].buttonPressed)
                {
                    gameObject.transform.position = groundSnapPoints[i].gameObject.transform.position;
                    gameObject.transform.parent = groundSnapPoints[i].gameObject.transform;
                    groundSnapPointButtons[i].Set_Backto_UnPressed();
                    break;
                }
            }
            // craft object
            else
            {
                if (groundSnapPointButtons[i].buttonPressed)
                {
                    if (Storage_Extra_Amount_Check(currentOpenedObject))
                    {
                        // dont use ingredients in place slots and decrease the amount in storage
                        Dispose_Object_inStorage(currentOpenedObject);
                        Storage_Text_Update(currentOpenedObject);

                        var craftedObject = Instantiate(currentOpenedObject.gameObjectPrefab, groundSnapPoints[i].transform);
                        craftedObject.transform.parent = groundSnapPoints[i].gameObject.transform;
                        groundSnapPointButtons[i].Set_Backto_UnPressed();

                        break;
                    }
                    else if (guestSystem.checkSystem.Object_Required_Ingreients_Check(currentOpenedObject))
                    {
                        // use ingredients in place slots
                        Use_Items_inSlots_forCraft(currentOpenedObject);

                        var craftedObject = Instantiate(currentOpenedObject.gameObjectPrefab, groundSnapPoints[i].transform);
                        craftedObject.transform.parent = groundSnapPoints[i].gameObject.transform;
                        groundSnapPointButtons[i].Set_Backto_UnPressed();

                        break;
                    }
                    else
                    {
                        Debug.Log("Not enough ingredients!");

                        break;
                    }
                }
            }
        }
        SnapPoint_Button_Availability_Update();
    }
    public void Wall_Object_Craft()
    {
        for (int i = 0; i < wallSnapPointButtons.Length; i++)
        {
            if (wallSnapPointButtons[i].buttonPressed)
            {
                if (Storage_Extra_Amount_Check(currentOpenedObject))
                {
                    // dont use ingredients in place slots and decrease the amount in storage
                    Dispose_Object_inStorage(currentOpenedObject);
                    Storage_Text_Update(currentOpenedObject);

                    var craftedObject = Instantiate(currentOpenedObject.gameObjectPrefab, wallSnapPoints[i].transform);
                    craftedObject.transform.parent = wallSnapPoints[i].gameObject.transform;
                    wallSnapPointButtons[i].Set_Backto_UnPressed();

                    break;
                }
                else if (guestSystem.checkSystem.Object_Required_Ingreients_Check(currentOpenedObject))
                {
                    // use ingredients in place slots
                    Use_Items_inSlots_forCraft(currentOpenedObject);

                    var craftedObject = Instantiate(currentOpenedObject.gameObjectPrefab, wallSnapPoints[i].transform);
                    craftedObject.transform.parent = wallSnapPoints[i].gameObject.transform;
                    wallSnapPointButtons[i].Set_Backto_UnPressed();

                    break;
                }
                else
                {
                    Debug.Log("Not enough ingredients!");

                    break;
                }
            }
        }
        SnapPoint_Button_Availability_Update();
    }
}
