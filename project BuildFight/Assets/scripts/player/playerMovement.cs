using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Movement_Input();
    }

    void FixedUpdate()
    {
        Single_Direction_Movement();
        Diagonal_Movement_SLowDown();
    }

    public float horizontal;
    public float vertical;
    void Movement_Input()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
    }

    public float speed;
    void Single_Direction_Movement()
    {
        rb.velocity = new Vector2(horizontal * speed, vertical * speed);
    }

    void Diagonal_Movement_SLowDown()
    {
        // top-right movement
        if (horizontal >= 1 && vertical >= 1)
        {
            rb.velocity = new Vector2(horizontal * speed, vertical * speed) / 1.3f;
        }

        // top-left movement
        if (horizontal <= -1 && vertical >= 1)
        {
            rb.velocity = new Vector2(horizontal * speed, vertical * speed) / 1.3f;
        }

        // bottom-right movement
        if (horizontal >= 1 && vertical <= -1)
        {
            rb.velocity = new Vector2(horizontal * speed, vertical * speed) / 1.3f;
        }

        // bottom-left movement
        if (horizontal <= -1 && vertical <= -1)
        {
            rb.velocity = new Vector2(horizontal * speed, vertical * speed) / 1.3f;
        }
    }
}
