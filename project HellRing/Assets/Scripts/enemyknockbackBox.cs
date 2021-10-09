using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyknockbackBox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var player = collision.GetComponent<PlayerMovement>();
            player.knockbackCount = player.knockbackLength;

            if (collision.transform.position.x < transform.position.x)
            {
                player.knockfromRight = true;
            }
            else
            {
                player.knockfromRight = false;
            }
        }
    }
}
