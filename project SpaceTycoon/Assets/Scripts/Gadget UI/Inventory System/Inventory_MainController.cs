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

    // craft jetpack test function
    private void Update()
    {
        Craft();
    }
    public void Craft()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            bag.Add_Item(backItems[0], 1);
            display.Update_Display();
        }
    }
}   