using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    public Player_MainController playerController;

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Movement_Input();
        Jump();
    }

    void FixedUpdate()
    {
        Movement();
    }

    [HideInInspector]
    public float horizontal;
    void Movement_Input()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
    }

    public float speed;
    void Movement()
    {
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }

    public float jumpForce;
    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }
}
