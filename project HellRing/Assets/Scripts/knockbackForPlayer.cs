using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class knockbackForPlayer : MonoBehaviour
{
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Enemy"))
            {
            var hit = collision.GetComponent<enemycontrolM1>();
                
                hit.knockbackCount = hit.knockbackLength;

                if (collision.transform.position.x < transform.position.x)
                {
                    hit.knockfromRight = true;
                }
                else
                {
                    hit.knockfromRight = false;
                }
            }
        } 
}
