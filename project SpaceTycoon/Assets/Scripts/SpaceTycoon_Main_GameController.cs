using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ObjectStorage
{
    public Object_ScrObj objectInfo;
    public int leftAmount;
}

public class SpaceTycoon_Main_GameController : MonoBehaviour
{
    // Main Game Variables
    public static bool isGroundOptionMenuOn = false;
    public static bool isWallOptionMenuOn = false;
    public static bool isPanelMenuOn = false;

    // In Game Conditions
    public static int EnginesOn = 0;
    public static int shipSectorLocation = 1;

    // Craftable Object Storage
    [SerializeField] private ObjectStorage[] _objectStorages;
    public ObjectStorage[] objectStorages => _objectStorages;

    // Object Icon Constructors
    public void Icon_Popup_UpdateCheck(bool playerDetection, GameObject objectIcon)
    {
        // icon on condition
        if (playerDetection && !isPanelMenuOn)
        {
            objectIcon.SetActive(true);
        }
        // icon off condition
        if (!playerDetection || isPanelMenuOn)
        {
            objectIcon.SetActive(false);
        }
    }
    public void Icon_Pressed(GameObject objectMenu)
    {
        objectMenu.SetActive(true);
        isPanelMenuOn = true;
    }

    // Object Main Panel Constructors
    public void Manual_TurnOff_ObjectPanel(GameObject objectMenu, GameObject[] objectOptions)
    {
        objectMenu.SetActive(false);
        TurnOff_All_Options_inObjectPanel(objectOptions);
        isPanelMenuOn = false;
        isGroundOptionMenuOn = false;
        isWallOptionMenuOn = false;
    }
    public void Automatic_TurnOff_ObjectPanel(bool playerDetection, GameObject objectMenu, GameObject[] objectOptions)
    {
        if (!playerDetection && objectMenu.activeSelf)
        {
            objectMenu.SetActive(false);
            TurnOff_All_Options_inObjectPanel(objectOptions);
            isPanelMenuOn = false;
            isGroundOptionMenuOn = false;
            isWallOptionMenuOn = false;
        }
    }

    // Object Options panel Constructors
    public void TurnOn_Single_Options_inObjectPanel(GameObject objectOptionMenu)
    {
        objectOptionMenu.SetActive(true);
        isGroundOptionMenuOn = true;
        isWallOptionMenuOn = true;
    }
    public void TurnOff_All_Options_inObjectPanel(GameObject[] optionList)
    {
        for (int i = 0; i < optionList.Length; i++)
        {
            optionList[i].SetActive(false);
        }
        isGroundOptionMenuOn = false;
        isWallOptionMenuOn = false;
    }

    // Ship Engine Constructors
    void Engines_Min_Set()
    {
        if (EnginesOn <= 0)
        {
            EnginesOn = 0;
        }
    }
    public void Engine_On_Pressed(GameObject onButton, GameObject offButton, GameObject engineLight)
    {
        onButton.SetActive(false);
        offButton.SetActive(true);
        engineLight.SetActive(true);
        EnginesOn += 1;
    }
    public void Engine_Off_Pressed(GameObject onButton, GameObject offButton, GameObject engineLight)
    {
        onButton.SetActive(true);
        offButton.SetActive(false);
        engineLight.SetActive(false);
        EnginesOn -= 1;
        Engines_Min_Set();
    }
}