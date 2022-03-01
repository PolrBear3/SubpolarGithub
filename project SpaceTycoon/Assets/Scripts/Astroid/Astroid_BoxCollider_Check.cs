using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astroid_BoxCollider_Check : MonoBehaviour
{
    BoxCollider2D bc;

    void Awake()
    {
        bc = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("astroid destroyer"))
        {
            Destroy(gameObject);
        }
    }
}
