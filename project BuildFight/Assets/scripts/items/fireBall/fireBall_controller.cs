using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireBall_controller : MonoBehaviour
{
    SpriteRenderer sr;
    public fireBall_main fireBall_main;

    bool scan;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        // start by disabling main script and the sprite renderer
        sr.enabled = false;
        fireBall_main.enabled = false;
    }

    void Update()
    {
        pickUp();
        Drop();
    }

    void pickUp()
    {
        // check if the player scanned the object, pressed E, and has a item on their hand
        if (scan == true && Input.GetKeyDown(KeyCode.E) && playerStatus.hasItem == false)
        {
            sr.enabled = true;
            fireBall_main.enabled = true;
            playerStatus.hasItem = true;
        }
    }

    void Drop()
    {
        // if the player press Q, empty hands
        if (Input.GetKeyDown(KeyCode.Q) && playerStatus.hasItem == true)
        {
            sr.enabled = false;
            fireBall_main.enabled = false;
            playerStatus.hasItem = false;

            // instantiate item fireBall
        }
    }

    //collision scan
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("fireBall"))
        {
            scan = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("fireBall"))
        {
            scan = false;
        }
    }
}
