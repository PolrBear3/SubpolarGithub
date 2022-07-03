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
    public snapPoint_Buttons[] groundSnapPointButtons;

    public snapPoint[] wallSnapPoints;
    public snapPoint_Buttons[] wallSnapPointButtons;

    public int openedObjOptionID;
    public Button_Detector[] objectCraftOptionButtons;
    public Object_ScrObj[] objectInfo;

    public GameObject mainPanel, optionPanel;
    public Image currentSelectedObjImage;
    public Text currentSelectedObjDescription;
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
        Storage_Text_Update();
    }

    private void Update()
    {
        controller.Icon_Popup_UpdateCheck(playerDetection, icon.gameObject);
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
    }
    public void Exit_Option()
    {
        controller.TurnOff_Single_Options_inObjectPanel(optionPanel);
        GroundSnapPoints_Off();
        WallSnapPoints_Off();
    }

    // Options SnapPoint ON and OFF
    void SnapPoint_Button_Availability_Update()
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

    void Automatic_SnapPoint_Off()
    {
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
    private void Current_Object_Preview(Object_ScrObj objectInfo)
    {
        currentSelectedObjImage.sprite = objectInfo.objectSprite;
        currentSelectedObjDescription.text = objectInfo.objectDescription;

        if (currentSelectedObjImage.sprite == null)
        {
            currentSelectedObjImage.sprite = deafultObjSprite;
            currentSelectedObjDescription.text = deafultObjDescription;
        }
    }
    public void Object_ID_Set(Object_ScrObj objectButtonScrObj) 
    {
        Exit_Option();
        SnapPoint_Button_Availability_Update();
        Current_Object_Preview(objectButtonScrObj);

        for (int i = 0; i < objectInfo.Length; i++)
        {
            if (objectButtonScrObj.objectID == objectInfo[i].objectID)
            {
                openedObjOptionID = objectInfo[i].objectID;
                controller.TurnOn_Single_Options_inObjectPanel(optionPanel);

                if (objectInfo[i].objectType == ObjectType.ground)
                {
                    GroundSnapPoints_On();
                }
                else if (objectInfo[i].objectType == ObjectType.wall)
                {
                    WallSnapPoints_On();
                }
                break;
            }
        }
    }
        // bool Player_has_Ingredients();
    public void Ground_Object_Craft()
    {
        for (int i = 0; i < groundSnapPointButtons.Length; i++)
        {
            // change crafttable position
            if (openedObjOptionID == 0)
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
                    Storage_Extra_Amount_Check();
                    var craftedObject = Instantiate(objectInfo[openedObjOptionID].gameObjectPrefab, groundSnapPoints[i].transform);
                    craftedObject.transform.parent = groundSnapPoints[i].gameObject.transform;
                    groundSnapPointButtons[i].Set_Backto_UnPressed();
                    break;
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
                Storage_Extra_Amount_Check();
                var craftedObject = Instantiate(objectInfo[openedObjOptionID].gameObjectPrefab, wallSnapPoints[i].transform);
                craftedObject.transform.parent = wallSnapPoints[i].gameObject.transform;
                wallSnapPointButtons[i].Set_Backto_UnPressed();
                break;
            }
        }
        SnapPoint_Button_Availability_Update();
    }
}
