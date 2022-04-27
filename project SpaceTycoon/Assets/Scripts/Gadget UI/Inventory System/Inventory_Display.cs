using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory_Display : MonoBehaviour
{
    public void Start()
    {
        Update_Display();
    }

    public Inventory_MainController controller;

    public void Update_Display()
    {
        Update_Bag_Display();
        Update_Inventory_Display();
    }

    // for bag display
    void Update_Bag_Display()
    {
        Reset_Bag_Display();

        for (int i = 0; i < controller.bag.storage.Count; i++)
        {
            var obj = Instantiate(controller.bag.storage[i].info.itemIcon, controller.bagSnapPoints);
            obj.GetComponentInChildren<Text>().text = controller.bag.storage[i].currentAmount.ToString();

            // check if icon current amount is less or equal to 0, and if 0, Destroy(gameObjetct);
            if (controller.bag.storage[i].currentAmount == 0)
            {
                Destroy(obj);
            }
        }
    }
    void Reset_Bag_Display()
    {
        foreach (Transform bagSnapIcon in controller.bagSnapPoints)
        {
            Destroy(bagSnapIcon.gameObject);
        }
    }

    // for inventory display
    void Update_Inventory_Display()
    {
        Reset_Inventory_Display();

        bool throwable1Taken = false;

        for (int i = 0; i < controller.inventory.storage.Count; i++)
        {
            // check the item type and place to that snappoint type
            if (controller.inventory.storage[i].info.itemType == ItemType.current)
            {
                var obj = Instantiate(controller.inventory.storage[i].info.itemIcon, controller.inventorySnapPoints[0]);
                obj.GetComponentInChildren<Text>().text = controller.inventory.storage[i].currentAmount.ToString();
            }
            else if(controller.inventory.storage[i].info.itemType == ItemType.back)
            {
                var obj = Instantiate(controller.inventory.storage[i].info.itemIcon, controller.inventorySnapPoints[1]);
                obj.GetComponentInChildren<Text>().text = controller.inventory.storage[i].currentAmount.ToString();
            }
            else if (controller.inventory.storage[i].info.itemType == ItemType.throwable)
            {
                if (!throwable1Taken)
                {
                    var obj = Instantiate(controller.inventory.storage[i].info.itemIcon, controller.inventorySnapPoints[2]);
                    obj.GetComponentInChildren<Text>().text = controller.inventory.storage[i].currentAmount.ToString();
                    throwable1Taken = true;
                }
                else if (throwable1Taken)
                {
                    var obj = Instantiate(controller.inventory.storage[i].info.itemIcon, controller.inventorySnapPoints[3]);
                    obj.GetComponentInChildren<Text>().text = controller.inventory.storage[i].currentAmount.ToString();
                }
            }
        }
    }
    void Reset_Inventory_Display()
    {
        for (int i = 0; i < controller.inventorySnapPoints.Length; i++)
        {
            foreach (Transform inventorySnapIcon in controller.inventorySnapPoints[i])
            {
                Destroy(inventorySnapIcon.gameObject);
            }
        }
    }
}