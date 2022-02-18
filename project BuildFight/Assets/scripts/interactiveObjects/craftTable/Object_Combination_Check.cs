using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Combination_Check : MonoBehaviour
{
    public CraftTable_Controller controller;
    
    public GameObject instantiatePoint;

    /// Ingredients///////////////////////////////////////////////////////////////////////
    public GameObject fireBall_slot1;
    public GameObject fireBall_slot2;

    public GameObject iceBall_slot1;
    public GameObject iceBall_slot2;

    public GameObject cane_slot1;
    public GameObject cane_slot2;

    public GameObject spear_slot1;
    public GameObject spear_slot2;

    /// Items to Instantiate///////////////////////////////////////////////////////////////////////
    public GameObject fireStaff;
    public GameObject fireSpear;

    public GameObject iceStaff;
    public GameObject iceSpear;
    
    /// ///////////////////////////////////////////////////////////////////////
    void Update()
    {
        Combinations();
    }

    void Combinations()
    {
        // fireStaff
        if (fireBall_slot1.activeSelf == true || fireBall_slot2.activeSelf == true)
        {
            if (cane_slot1.activeSelf == true || cane_slot2.activeSelf == true)
            {
                if (controller.slot1.slot1Empty == false && controller.slot2.slot2Empty == false)
                {
                    controller.anim.SetTrigger("craft");
                    Instantiate(fireStaff, instantiatePoint.transform.position, Quaternion.identity);
                    controller.slot1.slot1Empty = true;
                    controller.slot2.slot2Empty = true;
                }
            }
        }

        // fireSpear
        if (fireBall_slot1.activeSelf == true || fireBall_slot2.activeSelf == true)
        {
            if (spear_slot1.activeSelf == true || spear_slot2.activeSelf == true)
            {
                if (controller.slot1.slot1Empty == false && controller.slot2.slot2Empty == false)
                {
                    controller.anim.SetTrigger("craft");
                    Instantiate(fireSpear, instantiatePoint.transform.position, Quaternion.identity);
                    controller.slot1.slot1Empty = true;
                    controller.slot2.slot2Empty = true;
                }
            } 
        }

        // iceStaff
        if (iceBall_slot1.activeSelf == true || iceBall_slot2.activeSelf == true)
        {
            if (cane_slot1.activeSelf == true || cane_slot2.activeSelf == true)
            {
                if (controller.slot1.slot1Empty == false && controller.slot2.slot2Empty == false)
                {
                    controller.anim.SetTrigger("craft");
                    Instantiate(iceStaff, instantiatePoint.transform.position, Quaternion.identity);
                    controller.slot1.slot1Empty = true;
                    controller.slot2.slot2Empty = true;
                }
            }
        }

        // iceSpear
        if (iceBall_slot1.activeSelf == true || iceBall_slot2.activeSelf == true)
        {
            if (spear_slot1.activeSelf == true || spear_slot2.activeSelf == true)
            {
                if (controller.slot1.slot1Empty == false && controller.slot2.slot2Empty == false)
                {
                    controller.anim.SetTrigger("craft");
                    Instantiate(iceSpear, instantiatePoint.transform.position, Quaternion.identity);
                    controller.slot1.slot1Empty = true;
                    controller.slot2.slot2Empty = true;
                }
            }
        }
    }

}
