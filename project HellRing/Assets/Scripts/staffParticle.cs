using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class staffParticle : MonoBehaviour
{
    public float speed = 20f;
    public Rigidbody2D rb;
    public GameObject impactEffect;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("terrain"))
        {
            Destroy(gameObject);
            Instantiate(impactEffect, transform.position, transform.rotation);
        }
    }
}
