using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    public Player_MainController playerController;

    [HideInInspector]
    public Rigidbody2D rb;

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
        Check_if_Ground();
        Movement_Input();
        Default_Jump();
        Tiredness_Update();
    }

    void FixedUpdate()
    {
        Movement();
    }

    public void Freeze_Player()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }
    public void UnFreeze_Player()
    {
        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void Check_if_Ground()
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

    private void Movement_Input()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
    }
    private void Movement()
    {
        rb.velocity = new Vector2(horizontal * playerController.playerOutfit.currentOutfit.movementSpeed, rb.velocity.y);
    }

    void Default_Jump()
    {
        var addSize = playerController.playerOutfit.currentOutfit.tirednessAddSize;

        if (Input.GetKeyDown(KeyCode.W) && isGround)
        {
            rb.velocity = new Vector2(rb.velocity.x, playerController.playerOutfit.currentOutfit.jumpForce);

            // add tiredness if player jumps
            playerController.playerState.Add_State_Size(1, addSize);

            if (!playerController.playerState.State_CurrentlyNot_Max(1))
            {
                // if tiredness is max, subtract health
                playerController.playerState.Subtract_State_Size(0, addSize);
            }
        }
    }

    public bool isMoving()
    {
        if (rb.velocity.x != 0 && isGround) { return true; }
        else return false;
    }
    public bool isNotMoving()
    {
        if (rb.velocity.magnitude == 0 && isGround) { return true; }
        else return false;
    }

    private void Tiredness_Update()
    {
        var decreaseSize = playerController.playerOutfit.currentOutfit.tirednessDecreaseSize;
        var increaseSize = playerController.playerOutfit.currentOutfit.tirednessIncreaseSize;

        if (isNotMoving())
        {
            // decrease tiredness if player is not moving
            playerController.playerState.Decrease_State_Size(1, decreaseSize);
        }
        if (isMoving())
        {
            // increase tiredness if player moves
            playerController.playerState.Increase_State_Size(1, increaseSize);
            
            if (!playerController.playerState.State_CurrentlyNot_Max(1))
            {
                // if tiredness is max, decrease health
                playerController.playerState.Decrease_State_Size(0, increaseSize);
            }
        }
    }
}
