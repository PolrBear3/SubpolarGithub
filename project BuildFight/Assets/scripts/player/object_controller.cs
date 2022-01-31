using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class object_controller : MonoBehaviour
{
    public static bool hasItem;

    public GameObject melee;

    public GameObject fireBall;
    bool fireBall_scanner;

    public GameObject iceBall;
    bool iceBall_scanner;

    public GameObject cane;
    bool cane_scanner;

    public GameObject spear;
    bool spear_scanner;

    private void Update()
    {
        pickUp();
        drop();
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

    void pickUp()
    {
        if (Input.GetKeyDown(KeyCode.E) && hasItem == false)
        {
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
        }
    }

    void drop()
    {
        if (Input.GetKeyDown(KeyCode.Q) && hasItem == true)
        {
            hasItem = false;

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
    }
}
