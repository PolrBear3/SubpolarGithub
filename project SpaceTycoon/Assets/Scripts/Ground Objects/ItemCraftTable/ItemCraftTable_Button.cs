using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCraftTable_Button : MonoBehaviour
{
    public ItemCraftTable controller;
    
    public Item_Info thisItemInfo;

    public void Open_OptionPanel_forThisItem()
    {
        controller.Open_Item_OptionPanel(thisItemInfo);
    }
}
