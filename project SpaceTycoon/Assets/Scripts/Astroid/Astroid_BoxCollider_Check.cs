using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astroid_BoxCollider_Check : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("astroid destroyer"))
        {
            Destroy(gameObject);
        }
    }
}
