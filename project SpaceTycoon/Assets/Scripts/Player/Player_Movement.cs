using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    public Player_MainController playerController;

    Rigidbody2D rb;

    // horizontal movement variable
    [HideInInspector]
    public float horizontal;

    // ground check variables
    [HideInInspector]
    public bool isGround = false;
    public Transform foot;
    public float footRadius;
    public LayerMask groundLayer;

    // ?? jetpack movement check
    public GameObject JetPack;
    public JetPack jetPack;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Movement_Input();
        Check_if_Ground();
        Jump();
        JetPack_Fly();
    }

    void FixedUpdate()
    {
        Movement();
    }

    void Movement_Input()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
    }
    void Movement()
    {
        rb.velocity = new Vector2(horizontal * playerController.playerOutfit.currentOutfit.movementSpeed, rb.velocity.y);
    }
    public bool Player_is_Moving()
    {
        if (rb.velocity.x != 0 && JetPack.activeSelf == false || jetPack.outOfFuel == true) { return true; }
        else return false;
    }

    public void Jump()
    {
        if (Player_is_Jumping())
        {
            rb.velocity = new Vector2(rb.velocity.x, playerController.playerOutfit.currentOutfit.jumpForce);
        }
    }
    public bool Player_is_Jumping()
    {
        if (Input.GetKeyDown(KeyCode.W) && isGround && JetPack.activeSelf == false || jetPack.outOfFuel == true) { return true; }
        else return false;
    }

    public bool Player_is_Standing()
    {
        if (rb.velocity.magnitude == 0 && JetPack.activeSelf == false || jetPack.outOfFuel == true) { return true; }
        else return false;
    }

    void JetPack_Fly()
    {
        if (Input.GetKey(KeyCode.W) && JetPack.activeSelf == true && jetPack.outOfFuel == false)
        {
            jetPack.buttonPressed = true;
            rb.AddForce(Vector2.up * jetPack.flyForce);
        }
        else
        {
            jetPack.buttonPressed = false;
        }
    }

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
