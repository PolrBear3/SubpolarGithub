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
        Check_if_Ground();
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
        if (Input.GetKeyDown(KeyCode.W) && isGround)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    bool isGround = false;
    public Transform foot;
    public float footRadius;
    public LayerMask groundLayer;
    void Check_if_Ground()
    {
        Collider2D collider = Physics2D.OverlapCircle(foot.position, footRadius, groundLayer);

        if (collider != null)
        {
            isGround = true;
        }
        else
        {
            isGround = false;
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(foot.position, footRadius);
    }
}
