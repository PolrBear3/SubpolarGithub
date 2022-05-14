using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackSlot : MonoBehaviour
{
    private void Update()
    {
        Inventory_Slot_Status_Update();
    }

    public BoxSlot_UI boxSlotUI;

    public List<Item_Info> itemInfo = new List<Item_Info>();
    public List<GameObject> playerItems = new List<GameObject>();

    public void Inventory_Slot_Status_Update()
    {
        if (boxSlotUI.assignedBoxSlot != null)
        {
            for (int i = 0; i < itemInfo.Count; i++)
            {
                if (boxSlotUI.assignedBoxSlot.itemInfo == itemInfo[i])
                {
                    playerItems[i].SetActive(true);
                    Debug.Log("Searching");
                }
                else
                {
                    playerItems[i].SetActive(false);
                    Debug.Log("Deactivating");
                }
            }
        }
    }
}
