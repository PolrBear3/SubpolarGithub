using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory_Display : MonoBehaviour
{
    public void Start()
    {
        Create_Bag_Display();
    }

    public Inventory_MainController controller;

    void Reset_Bag_Display()
    {
        foreach (Transform childTransform in controller.bagSnapPoints)
        {
            Destroy(childTransform.gameObject);
        }
    }

    public void Create_Bag_Display()
    {
        Reset_Bag_Display();

        for (int i=0; i<controller.bag.storage.Count; i++)
        {
            var obj = Instantiate(controller.bag.storage[i].info.itemIcon, controller.bagSnapPoints);
            obj.GetComponentInChildren<Text>().text = controller.bag.storage[i].currentAmount.ToString();
        }

        // if icon current amount is less or equal to 0, Destroy(gameObjetct);
    }
}