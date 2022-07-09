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
    // Main Menu UI Control
    public bool isPanelMenuOn = false;
    public int multitaskAvailable = 2;

    // In Game Conditions
    public int EnginesOn = 0;
    public int shipSectorLocation = 1;

    // Craftable Object Storage
    [SerializeField] private ObjectStorage[] _objectStorages;
    public ObjectStorage[] objectStorages => _objectStorages;

    // All Player Item Informations
    [SerializeField] private Item_Info[] _items;
    public Item_Info[] items => _items;

    [SerializeField] private Item_Info[] _ingredients;
    public Item_Info[] ingredients => _ingredients;

    // Object Icon Constructors
    public void Icon_Popup_UpdateCheck(bool playerDetection, GameObject objectIcon)
    {
        // icon on condition
        if (playerDetection && !isPanelMenuOn && multitaskAvailable > 0)
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
        multitaskAvailable -= 1;
    }

    // Object Main Panel Constructors
    public void Manual_TurnOff_ObjectPanel(GameObject objectMenu)
    {
        objectMenu.SetActive(false);
        isPanelMenuOn = false;
        multitaskAvailable += 1;
    }
    public void Automatic_TurnOff_ObjectPanel(bool playerDetection, GameObject objectMenu)
    {
        if (!playerDetection && objectMenu.activeSelf)
        {
            objectMenu.SetActive(false);
            isPanelMenuOn = false;
            multitaskAvailable += 1;
        }
    }

    // Object Options panel Constructors
    public void TurnOn_Single_Options_inObjectPanel(GameObject objectMenu)
    {
        objectMenu.SetActive(true);
    }
    public void TurnOff_Single_Options_inObjectPanel(GameObject objectMenu)
    {
        objectMenu.SetActive(false);
    }

    public void Manual_TurnOff_All_Options_inObjectPanel(GameObject[] objectOptions)
    {
        for (int i = 0; i < objectOptions.Length; i++)
        {
            objectOptions[i].SetActive(false);
        }
    }
    public void Automatic_TurnOff_All_Options_inObjectPanel(bool playerDetection, GameObject[] objectOptions)
    {
        if (!playerDetection)
        {
            for (int i = 0; i < objectOptions.Length; i++)
            {
                objectOptions[i].SetActive(false);
            }
        }
    }
    public void Automatic_TurnOff_Single_Options_inObjectPanel(bool playerDetection, GameObject objectOption)
    {
        if (!playerDetection)
        {
            objectOption.SetActive(false);
        }
    }

    // Object Default Function Constructors
    public void Object_Dismantle(Object_ScrObj objectInfo, int storageStoreAmount, Icon icon, GameObject thisGameObject)
    {
        for (int i = 0; i < objectStorages.Length; i++)
        {
            if (objectStorages[i].objectInfo == objectInfo)
            {
                objectStorages[i].leftAmount += storageStoreAmount;
                icon.iconBoxCollider.SetActive(false);
                Destroy(thisGameObject);
                isPanelMenuOn = false;
                multitaskAvailable += 1;
                break;
            }
        }
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