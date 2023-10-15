using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObject_Item_Info : MonoBehaviour
{
    public Equip_Slot equipSlot;
    public Item_Info itemInfo;
    public GameObject systemGameObject;

    public void Activate_Item()
    {
        systemGameObject.SetActive(true);
    }
    public void DeActivate_Item()
    {
        systemGameObject.SetActive(false);
    }
}
