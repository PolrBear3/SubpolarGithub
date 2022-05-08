using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gadget_MainController : MonoBehaviour
{
    public GameObject[] menus;

    public void Close_All_Menu()
    {
        for (int i=0; i < menus.Length; i++)
        {
            if (menus[i].activeSelf && !Mouse_Item_Icon.holdingItemOnMouse)
            {
                menus[i].SetActive(false);
                break;
            }
        }
    }
}
