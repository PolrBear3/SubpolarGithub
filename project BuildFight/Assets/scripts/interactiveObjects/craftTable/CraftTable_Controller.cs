using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftTable_Controller : MonoBehaviour
{
    public slot1_Display slot1;
    public slot2_Display slot2;

    public BoxCollider2D slot1_bc;
    public BoxCollider2D slot2_bc;

    public GameObject hand1;
    public object_controller playerObjectController;

    public GameObject hand2;
    public second_object_controller playerSecondObjectController;
  
    void Start()
    {
        slot2_bc.enabled = false;
    }

    void Update()
    {
        Each_Slot_Fill_Check();
    }

    void Each_Slot_Fill_Check()
    {
        if (slot1.slot1Empty == false)
        {
            slot1_bc.enabled = false;
            slot2_bc.enabled = true;
        }

        if (slot1.slot1Empty == true)
        {
            slot1_bc.enabled = true;
            slot2_bc.enabled = false;
        }
    }
}
