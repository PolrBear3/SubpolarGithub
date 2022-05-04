using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Inventory : MonoBehaviour
{
    public List<Item_Info> items = new List<Item_Info>();

    private void Update()
    {
        Add_JetPack();
    }

    void Add_JetPack()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            var boxHolder = GetComponent<Box_Holder>();
            boxHolder.boxSystem.Add_to_Box(items[0], 1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            var boxHolder = GetComponent<Box_Holder>();
            boxHolder.boxSystem.Add_to_Box(items[1], 1);
        }
    }
}
