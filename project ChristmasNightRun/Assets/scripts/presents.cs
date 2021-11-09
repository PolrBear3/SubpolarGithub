using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class presents : MonoBehaviour
{
    public float speed;
    public Rigidbody2D rb;
    [SerializeField] float rotSpeed;

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
