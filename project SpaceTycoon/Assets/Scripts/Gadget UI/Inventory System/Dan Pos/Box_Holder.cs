using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Box_Holder : MonoBehaviour
{   
    public Player_Inventory controller;

    private void Awake()
    {
        _boxSystem = new Box_System(boxSize);
    }

    [SerializeField] private int boxSize;
    [SerializeField] protected Box_System _boxSystem;
    public Box_System boxSystem => _boxSystem;

    public static UnityAction<Box_System> boxSlotUpdateRequested;


    // craft item function
    public List<Item_Info> items = new List<Item_Info>();

    public void Craft_Item(int itemNum, int amount)
    {
        _boxSystem.Add_to_Box(items[itemNum], amount);
    }
}
