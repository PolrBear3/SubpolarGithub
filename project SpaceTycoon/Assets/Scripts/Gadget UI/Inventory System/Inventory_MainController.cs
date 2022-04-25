using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_MainController : MonoBehaviour
{
    public Inventory inventory;
    public Bag bag;

    public Inventory_Display display;

    public Transform bagSnapPoints;

    // for craft table item lists
    public Default_Info[] currentItems;
    public Default_Info[] backItems;
    public Default_Info[] throwableItems;
    public Default_Info[] ingredients;

    // test functions
    private void Update()
    {
        Craft_Ceres();
    }
    public void Craft_Ceres()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            bag.Add_Item(ingredients[0], 1);
            display.Create_Bag_Display();
        }
    }

    // public void subtract ceres
}   
