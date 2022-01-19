using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireBall_dropped : MonoBehaviour
{
    bool scanned = false;
    
    private void Update()
    {
        if (scanned == true && Input.GetKeyDown(KeyCode.E))
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("hand"))
        {
            scanned = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("hand"))
        {
            scanned = false;
        }
    }
}
