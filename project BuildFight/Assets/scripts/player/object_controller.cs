using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class object_controller : MonoBehaviour
{
    public static bool hasItem;

    public GameObject melee;

    /// Ingredients///////////////////////////////////////////////////////////////////////
    public GameObject fireBall;
    bool fireBall_scanner;

    public GameObject iceBall;
    bool iceBall_scanner;

    public GameObject cane;
    bool cane_scanner;

    public GameObject spear;
    bool spear_scanner;

    /// Items///////////////////////////////////////////////////////////////////////
    public GameObject fireStaff;
    bool fireStaff_scanner;

    public GameObject fireSpear;
    bool fireSpear_scanner;

    public GameObject iceStaff;
    bool iceStaff_scanner;

    public GameObject iceSpear;
    bool iceSpear_scanner;

    private void Update()
    {
        PickUp();
        Drop();
        Player_Default_State();
    }

    void Player_Default_State()
    {
        if (hasItem == false)
        {
            melee.SetActive(true);
        }
        else if (hasItem == true)
        {
            melee.SetActive(false);
        }
    }

    void PickUp()
    {
        if (Input.GetKeyDown(KeyCode.E) && hasItem == false)
        {
            /// Ingredients///////////////////////////////////////////////////////////////////////
            if (fireBall_scanner == true)
            {
                fireBall.SetActive(true);
                fireBall_scanner = false;
                hasItem = true;
            }

            if (iceBall_scanner == true)
            {
                iceBall.SetActive(true);
                iceBall_scanner = false;
                hasItem = true;
            }

            if (cane_scanner == true)
            {
                cane.SetActive(true);
                cane_scanner = false;
                hasItem = true;
            }

            if (spear_scanner == true)
            {
                spear.SetActive(true);
                spear_scanner = false;
                hasItem = true;
            }
            
            /// Items///////////////////////////////////////////////////////////////////////
            if (fireStaff_scanner == true)
            {
                fireStaff.SetActive(true);
                fireStaff_scanner = false;
                hasItem = true;
            }
            
            if (fireSpear_scanner == true)
            {
                fireSpear.SetActive(true);
                fireSpear_scanner = false;
                hasItem = true;
            }

            if (iceStaff_scanner == true)
            {
                iceStaff.SetActive(true);
                iceStaff_scanner = false;
                hasItem = true;
            }

            if (iceSpear_scanner == true)
            {
                iceSpear.SetActive(true);
                iceSpear_scanner = false;
                hasItem = true;
            }
        }
    }

    void Drop()
    {
        if (Input.GetKeyDown(KeyCode.Q) && hasItem == true)
        {
            hasItem = false;

            /// Ingredients///////////////////////////////////////////////////////////////////////
            if (fireBall.activeSelf == true)
            {
                fireBall.SetActive(false);
            }

            if (iceBall.activeSelf == true)
            {
                iceBall.SetActive(false);
            }

            if (cane.activeSelf == true)
            {
                cane.SetActive(false);
            }

            if (spear.activeSelf == true)
            {
                spear.SetActive(false);
            }

            /// Items///////////////////////////////////////////////////////////////////////
            if (fireStaff.activeSelf == true)
            {
                fireStaff.SetActive(false);
            }

            if (fireSpear.activeSelf == true)
            {
                fireSpear.SetActive(false);
            }

            if (iceStaff.activeSelf == true)
            {
                iceStaff.SetActive(false);
            }

            if (iceSpear.activeSelf == true)
            {
                iceSpear.SetActive(false);
            }
        }
    }

    // scanner /////////////////////////////////////////////////////////
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("fireBall"))
        {
            fireBall_scanner = true;
        }
        if (collision.CompareTag("iceBall"))
        {
            iceBall_scanner = true;
        }
        if (collision.CompareTag("cane"))
        {
            cane_scanner = true;
        }
        if (collision.CompareTag("spear"))
        {
            spear_scanner = true;
        }
        if (collision.CompareTag("fireStaff"))
        {
            fireStaff_scanner = true;
        }
        if (collision.CompareTag("fireSpear"))
        {
            fireSpear_scanner = true;
        }
        if (collision.CompareTag("iceStaff"))
        {
            iceStaff_scanner = true;
        }
        if (collision.CompareTag("iceSpear"))
        {
            iceSpear_scanner = true;
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("fireBall"))
        {
            fireBall_scanner = false;
        }
        if (collision.CompareTag("iceBall"))
        {
            iceBall_scanner = false;
        }
        if (collision.CompareTag("cane"))
        {
            cane_scanner = false;
        }
        if (collision.CompareTag("spear"))
        {
            spear_scanner = false;
        }
        if (collision.CompareTag("fireStaff"))
        {
            fireStaff_scanner = false;
        }
        if (collision.CompareTag("fireSpear"))
        {
            fireSpear_scanner = false;
        }
        if (collision.CompareTag("iceStaff"))
        {
            iceStaff_scanner = false;
        }
        if (collision.CompareTag("iceSpear"))
        {
            iceSpear_scanner = false;
        }
    }
}
