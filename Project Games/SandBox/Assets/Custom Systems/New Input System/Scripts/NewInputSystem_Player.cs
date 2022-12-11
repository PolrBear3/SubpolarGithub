using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewInputSystem_Player : MonoBehaviour
{
    private Rigidbody2D rb;

    private Vector2 moveDirection;
    public float moveSpeed;
    public float jumpForce;

    private void Awake()
    {
        Connection();
    }
    private void FixedUpdate()
    {
        Move();
    }

    private void Connection()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void OnMove(InputValue value)
    {
        moveDirection = value.Get<Vector2>();
    }
    private void Move()
    {
        rb.velocity = new Vector2(moveDirection.x * moveSpeed, rb.velocity.y);
    }

    public void OnJump()
    {
        Jump();
    }
    private void Jump()
    {
        rb.AddForce(Vector2.up * (jumpForce * 10));
    } 
}
