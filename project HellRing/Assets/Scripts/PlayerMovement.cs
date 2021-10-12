using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;
    public static float m;    
    private bool pickupAllow;
    bool facing = true;
    [SerializeField] private float movespeed;
    private enum MovementState { idle, walk }
    public static int playermaxHealth = 100;
    public static int playercurrentHealth;

    // knockback
    public float knockback;
    public float knockbackLength;
    public float knockbackCount;
    public bool knockfromRight;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = this.GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        playercurrentHealth = playermaxHealth;
    }

    void Update()
    {
        // Horizontal movement update
        m = Input.GetAxisRaw("Horizontal");
        
        if (knockbackCount <= 0)
        {
            rb.velocity = new Vector2(m * movespeed, rb.velocity.y);
        }
        else
        {
            if (knockfromRight)
            {
                rb.velocity = new Vector2(-knockback, knockback);
            }
            if (!knockfromRight)
            {
                rb.velocity = new Vector2(knockback, knockback);
            }
            knockbackCount -= Time.deltaTime;
        }
        
        // player flipping update
        if(m>0 && !facing)
        {
            flip();
        }
        else if (m<0 && facing)
        {
            flip();
        }

        // animation update
        UpdateAnimationUpdate(); 

        // for player picking up item
        if (pickupAllow && Input.GetKeyDown(KeyCode.E))
        {
            this.sr.enabled = false;    // turning off noWeaponPlayer sprite renderer after player pickup weapon
        }

        // player health check 
        Debug.Log("Player current health: " + playercurrentHealth);

        // player condition
        if (playercurrentHealth <= 0)
        {
            Debug.Log("Player died");
        }
    }
    
    // player flip
    void flip()
    {
        facing = !facing;
        transform.Rotate(0f, 180f, 0f);
    }

    // player movement animation
    private void UpdateAnimationUpdate()
    {
        MovementState state;

        if (m > 0f)
        {
            state = MovementState.walk;
        }
        else if (m < 0f)
        {
            state = MovementState.walk;      
        }
        else
        {
            state = MovementState.idle;
        }

        anim.SetInteger("state", (int)state);
    }  
    
    // choose item pickup function
    private void OnTriggerEnter2D(Collider2D collision)
    { 
        if (collision.CompareTag("swordBox") || collision.CompareTag("staffBox") || collision.CompareTag("hammerBox"))
        {
            pickupAllow = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("swordBox") || collision.CompareTag("staffBox") || collision.CompareTag("hammerBox"))
        {
            pickupAllow = false;
        }
    }
}

