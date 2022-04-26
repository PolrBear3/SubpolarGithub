using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_MainController : MonoBehaviour
{
    // player bag and inventory scriptable objects
    public Inventory inventory;
    public Bag bag;

    // display script connection
    public Inventory_Display display;

    // bag and inventory snap points
    public Transform bagSnapPoints;
    public Transform[] inventorySnapPoints;

    // for craft table item lists
    public Default_Info[] currentItems;
    public Default_Info[] backItems;
    public Default_Info[] throwableItems;
    public Default_Info[] ingredients;

    // craftng and subtracting test function
    private void Update()
    {
        Craft();
        Subtract();
    }
    public void Craft()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            inventory.Add_Item(backItems[0], 1);
            display.Update_Display();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            bag.Add_Item(backItems[0], 1);
            display.Update_Display();
        }
    }
    public void Subtract()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            inventory.Subtract_Item(backItems[0], 1);
            display.Update_Display();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            bag.Subtract_Item(backItems[0], 1);
            display.Update_Display();
        }
    }
}   