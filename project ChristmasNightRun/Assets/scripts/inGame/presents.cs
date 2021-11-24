using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class presents : MonoBehaviour
{
    float speed = 2f;
    private Rigidbody2D rb;
    float rotSpeed = 1.5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;
    }

    void Update()
    {
        transform.Rotate(new Vector3(0, 0, -rotSpeed));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("target") || collision.CompareTag("destroyBox"))
        {
            Destroy(gameObject);
        }
    }
}
