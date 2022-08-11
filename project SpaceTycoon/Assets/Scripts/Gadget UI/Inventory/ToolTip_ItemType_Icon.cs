using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolTip_ItemType_Icon : MonoBehaviour
{
    public Sprite[] itemTypeIcons;
    
    public Sprite currentItemTypeIcon;

    public void Set_ItemType_Icon(ItemType itemType)
    {
        if (itemType == ItemType.throwable)
        {
            currentItemTypeIcon = itemTypeIcons[0];
        }
        else if (itemType == ItemType.back)
        {
            currentItemTypeIcon = itemTypeIcons[1];
        }
        else if (itemType == ItemType.hand)
        {
            currentItemTypeIcon = itemTypeIcons[2];
        }
    }
}
