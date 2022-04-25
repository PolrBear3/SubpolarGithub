using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_MainController : MonoBehaviour
{
    public Inventory inventory;
    public Bag bag;

    public Transform bagSnapPoints;

    // for craft table item lists
    public Default_Info[] currentItems;
    public Default_Info[] backItems;
    public Default_Info[] throwableItems;
    public Default_Info[] ingredients;

    // test functions
    private void Update()
    {
        Craft_JetPack();
    }
    public void Craft_JetPack()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            bag.Add_Item(backItems[0], 1);
        }
    }
}   
