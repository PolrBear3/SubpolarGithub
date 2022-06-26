using UnityEngine;
using UnityEngine.UI;

public class EscapePod_CraftTable : MonoBehaviour
{
    SpaceTycoon_Main_GameController controller;

    Animator anim;
    [HideInInspector]
    public bool playerDetection;

    // gameobject ground and wall snappoints
    public snapPoint[] groundSnapPoints;
    public GameObject[] groundSnapPointButtons;
    public Button_Detector[] groundButtons;

    public snapPoint[] wallSnapPoints;
    public GameObject[] wallSnapPointButtons;
    public Button_Detector[] wallButtons;

    public GameObject[] optionMenus;

    public int openedObjOptionID;
    public Button_Detector[] objectCraftOptionButtons;
    public Object_ScrObj[] objectInfo;

    public Text[] objectCurrentStorageText;
    public GameObject mainPanel;
    public Icon icon;

    private void Awake()
    {
        controller = GameObject.FindGameObjectWithTag("SpaceTycoon Main GameController").GetComponent<SpaceTycoon_Main_GameController>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        SnapPoint_ObjectStorage_Availability_Update();
        GroundSnapPoints_Sprite_Off();
        WallSnapPoints_Sprite_Off();
        Storage_Text_Update();
    }

    private void Update()
    {
        controller.Icon_Popup_UpdateCheck(playerDetection, icon.gameObject);
        controller.Automatic_TurnOff_ObjectPanel(playerDetection, mainPanel);
        controller.Automatic_TurnOff_All_Options_inObjectPanel(playerDetection, optionMenus);
        Automatic_SnapPoint_Sprite_Off();
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
    public void Open_MainPanel()
    {
        controller.Icon_Pressed(mainPanel);
    }
    public void Exit_Menu()
    {
        controller.Manual_TurnOff_ObjectPanel(mainPanel);
        controller.Manual_TurnOff_All_Options_inObjectPanel(optionMenus);
        GroundSnapPoints_Sprite_Off();
        WallSnapPoints_Sprite_Off();
    }
    public void Exit_Option()
    {
        controller.Manual_TurnOff_All_Options_inObjectPanel(optionMenus);
        GroundSnapPoints_Sprite_Off();
        WallSnapPoints_Sprite_Off();
    }

    // Options SnapPoint Sprite ON and OFF
    void SnapPoint_ObjectStorage_Availability_Update()
    {
        // snappoint update
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

        // object current storage text update
        for (int i = 0; i < objectCurrentStorageText.Length; i++)
        {
            objectCurrentStorageText[i].text = controller.objectStorages[i].leftAmount.ToString();
        }
    }
    void Automatic_SnapPoint_Sprite_Off()
    {
        if (!playerDetection)
        {
            GroundSnapPoints_Sprite_Off();
            WallSnapPoints_Sprite_Off();
        }
    }

    public void GroundSnapPoints_Sprite_On()
    {
        for (int i = 0; i < groundSnapPoints.Length; i++)
        {
            groundSnapPoints[i].Sprite_On();
        }
        SnapPoint_ObjectStorage_Availability_Update();
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
        SnapPoint_ObjectStorage_Availability_Update();
    }
    public void WallSnapPoints_Sprite_Off()
    {
        for (int i = 0; i < wallSnapPoints.Length; i++)
        {
            wallSnapPoints[i].Sprite_Off();
        }
    }

    // storage system
    void Storage_Extra_Amount_Check()
    {
        if (controller.objectStorages[openedObjOptionID].leftAmount != 0)
        {
            controller.objectStorages[openedObjOptionID].leftAmount -= 1;
            Storage_Text_Update();
        }
        else if (controller.objectStorages[openedObjOptionID].leftAmount == 0)
        {
            // spend ingredients
        }
    }

    public void Storage_Text_Update()
    {
        for (int i = 0; i < objectCurrentStorageText.Length; i++)
        {

            objectCurrentStorageText[i].text = controller.objectStorages[i].leftAmount.ToString();
        }
    }
    public void Refund_Object_forIngredients(Object_ScrObj objectInfo)
    {
        for (int i = 0; i < controller.objectStorages.Length; i++)
        {
            if (controller.objectStorages[i].objectInfo == objectInfo)
            {
                if (controller.objectStorages[i].leftAmount != 0)
                {
                    controller.objectStorages[i].leftAmount -= 1;
                    Storage_Text_Update();
                }

                if (controller.objectStorages[i].leftAmount == 0)
                {
                    controller.objectStorages[i].leftAmount = 0;
                    // does not give ingredients
                }
            }
        }
    }

    // Options and Craft Functions
    public void Object_ID_Set(Object_ScrObj objectButtonScrObj)
    {
        Exit_Option();
        SnapPoint_ObjectStorage_Availability_Update();
        for (int i = 0; i < objectInfo.Length; i++)
        {
            if (objectButtonScrObj.objectID == objectInfo[i].objectID)
            {
                openedObjOptionID = objectInfo[i].objectID;

                if (objectInfo[i].objectType == ObjectType.ground)
                {
                    controller.TurnOn_Single_Options_inObjectPanel(optionMenus[0]);
                    GroundSnapPoints_Sprite_On();
                }
                else if (objectInfo[i].objectType == ObjectType.wall)
                {
                    controller.TurnOn_Single_Options_inObjectPanel(optionMenus[1]);
                    WallSnapPoints_Sprite_On();
                }
                break;
            }
        }
    }
        // bool Player_has_Ingredients();
    public void Ground_Object_Craft()
    {
        for (int i = 0; i < groundButtons.Length; i++)
        {
            // change crafttable position
            if (openedObjOptionID == 0)
            {
                if (groundButtons[i].buttonPressed)
                {
                    gameObject.transform.position = groundSnapPoints[i].gameObject.transform.position;
                    gameObject.transform.parent = groundSnapPoints[i].gameObject.transform;
                    groundButtons[i].Set_Backto_UnPressed();
                    break;
                }
            }
            // craft object
            else
            {
                if (groundButtons[i].buttonPressed)
                {
                    Storage_Extra_Amount_Check();
                    var craftedObject = Instantiate(objectInfo[openedObjOptionID].gameObjectPrefab, groundSnapPoints[i].transform);
                    craftedObject.transform.parent = groundSnapPoints[i].gameObject.transform;
                    groundButtons[i].Set_Backto_UnPressed();
                    break;
                }
            }
        }
        SnapPoint_ObjectStorage_Availability_Update();
    }
    public void Wall_Object_Craft()
    {
        for (int i = 0; i < wallButtons.Length; i++)
        {
            if (wallButtons[i].buttonPressed)
            {
                Storage_Extra_Amount_Check();
                var craftedObject = Instantiate(objectInfo[openedObjOptionID].gameObjectPrefab, wallSnapPoints[i].transform);
                craftedObject.transform.parent = wallSnapPoints[i].gameObject.transform;
                wallButtons[i].Set_Backto_UnPressed();
                break;
            }
        }
        SnapPoint_ObjectStorage_Availability_Update();
    }
}
