using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gadget_MainController : MonoBehaviour
{
    public GameObject[] menus;

    public void Close_All_Gadget_Menus(Animator openMenuButton)
    {
        for (int i=0; i < menus.Length; i++)
        {
            if (!Mouse_Item_Icon.holdingItemOnMouse)
            {
                menus[i].SetActive(false);
                openMenuButton.SetBool("isPressed", false);
                break;
            }
        }
    }

    public void Open_Gadget_Menu(GameObject menu, Animator openMenuButton)
    {
        if (!Mouse_Item_Icon.holdingItemOnMouse)
        {
            Close_All_Gadget_Menus(openMenuButton);
            menu.SetActive(true);
            openMenuButton.SetBool("isPressed", true);
        }
    }
}
