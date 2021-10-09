using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class theFalsePresenceDetector : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            theFalsePresence.melee = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            theFalsePresence.melee = false;
        }
    } 
}
