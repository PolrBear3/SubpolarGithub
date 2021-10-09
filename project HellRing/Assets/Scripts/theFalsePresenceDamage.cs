using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class theFalsePresenceDamage : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovement.playercurrentHealth -= 10;  // damage
        }
    }
}
