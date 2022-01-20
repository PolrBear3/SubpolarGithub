using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class object_controller : MonoBehaviour
{
    public static bool hasItem;

    public GameObject fireBall;
    bool fireBall_scanner;

    public GameObject iceBall;
    bool iceBall_scanner;

    private void Update()
    {
        // pickup function /////////////////////////////////////////////////////////
        if (Input.GetKeyDown(KeyCode.E) && hasItem == false)
        {
            // pickup fireball
            if (fireBall_scanner == true)
            {
                fireBall.SetActive(true);
                fireBall_scanner = false;
                hasItem = true;
            }
            
            // pickup iceball
            if (iceBall_scanner == true)
            {
                iceBall.SetActive(true);
                iceBall_scanner = false;
                hasItem = true;
            }
        }

        // drop function /////////////////////////////////////////////////////////
        if (Input.GetKeyDown(KeyCode.Q) && hasItem == true)
        {
            hasItem = false;

            // drop fireball
            if (fireBall.activeSelf == true)
            {
                fireBall.SetActive(false);
            }

            // drop iceball
            if (iceBall.activeSelf == true)
            {
                iceBall.SetActive(false);
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
    }
}
