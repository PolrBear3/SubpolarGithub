using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    private void Update()
    {
        if (Time.time > nextDropTime)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Drop();
                nextDropTime = Time.time + 1f / dropRate;
            }
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (isGrounded)
            {
                Jump();
            }
        }
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    public Transform dropPoint;
    public GameObject present;
    float dropRate = 2f;
    float nextDropTime = 0f;
    void Drop()
    {
        Instantiate(present, dropPoint.position, dropPoint.rotation);
    }
    
    public float jumphight;
    bool isGrounded;
    public Transform groundCheck;
    public LayerMask groundLayer;
    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumphight);
    }
}
