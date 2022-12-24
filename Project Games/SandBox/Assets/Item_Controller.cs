using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuffable
{
    void Give_Buff();
}

public class Item_Controller : MonoBehaviour
{
    public GameObject[] items;

    public void Give_Buff_All()
    {
        for (int i = 0; i < items.Length; i++)
        {
            var item = items[i].GetComponent<IBuffable>();

            item.Give_Buff();
        }
    }
}
