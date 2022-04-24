using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_MainController : MonoBehaviour
{
    private void Update()
    {
        Craft_JetPack();
        Craft_Drill();
        Craft_Ceres();
    }

    public Bag playerBag;

    public Default_Info[] itemInfoData;

    void Craft_JetPack()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            playerBag.Add_Item(itemInfoData[0], 1);
        }
    }

    void Craft_Drill()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            playerBag.Add_Item(itemInfoData[1], 1);
        }
    }

    void Craft_Ceres()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            playerBag.Add_Item(itemInfoData[2], 1);
        }
    }
}
