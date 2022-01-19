using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireBall_controller : MonoBehaviour
{
    public fireBall_main fireBall_main;
    
    SpriteRenderer sr;
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
            playerStatus.hasItem = true;
            hands.holding = true;

            sr.enabled = true;
            fireBall_main.enabled = true;
        }
    }

    void Drop()
    {
        // if the player press Q, empty hands
        if (Input.GetKeyDown(KeyCode.Q) && playerStatus.hasItem == true)
        {
            playerStatus.hasItem = false;
            hands.holding = false;

            sr.enabled = false;
            fireBall_main.enabled = false;
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
