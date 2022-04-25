using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory_Display : MonoBehaviour
{
    public Inventory_MainController controller;

    private void Start()
    {
        Create_Bag_Display();
    }

    public void Create_Bag_Display()
    {
        for (int i=0; i<controller.bag.storage.Count; i++)
        {
            var obj = Instantiate(controller.bag.storage[i].info.itemIcon, controller.bagSnapPoints);
            obj.GetComponentInChildren<Text>().text = controller.bag.storage[i].currentAmount.ToString();
        }
    }

    public void Update_Bag_Display()
    {

    }
}