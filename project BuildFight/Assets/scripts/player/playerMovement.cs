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
        Movement();
    }

    [HideInInspector]
    public float horizontal;
    [HideInInspector]
    public float vertical;
    private Vector2 moveDirection;
    void Movement_Input()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector2(horizontal, vertical).normalized;
    }

    public float speed;
    void Movement()
    {
        rb.velocity = new Vector2(moveDirection.x * speed, moveDirection.y * speed);
    }
}
