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
        Check_if_Ground();
        Jump();
        JetPack_Fly();
        Check_if_Sitting_or_Sleeping();
    }

    void FixedUpdate()
    {
        Movement();
    }

    // horizontal movement
    [HideInInspector]
    public float horizontal;
    void Movement_Input()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
    }
    void Movement()
    {
        rb.velocity = new Vector2(horizontal * playerController.playerOutfit.currentOutFit.movementSpeed, rb.velocity.y);
    }

    // jump vertical movement and JetPack fly
    public GameObject JetPack;
    public JetPack jetPack;
    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.W) && isGround)
        {
            if (JetPack.activeSelf == false || jetPack.outOfFuel == true)
            {
                rb.velocity = new Vector2(rb.velocity.x, playerController.playerOutfit.currentOutFit.jumpForce);
            }
        }
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

    [HideInInspector]
    public bool isGround = false;
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

    // sit sleep check
    void Check_if_Sitting_or_Sleeping()
    {
        if (Player_State.player_isSitting || Player_State.player_isSleeping)
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionY;
        }
        if (Player_State.player_isMoving)
        {
            rb.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation; 
        }
    }
}
