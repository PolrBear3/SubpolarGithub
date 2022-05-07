using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Inventory : MonoBehaviour
{
    private void Update()
    {
        Add_JetPack();
    }

    // item craft test
    public List<Item_Info> items = new List<Item_Info>();

    void Add_JetPack()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            var boxHolder = GetComponent<Box_Holder>();
            boxHolder.boxSystem.Add_to_Box(items[0], 1);
        }
    }
}
