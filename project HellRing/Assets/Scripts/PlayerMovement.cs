using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;
    private float m;    
    private bool pickupAllow;
    bool facing = true;
    [SerializeField] private float movespeed;
    private enum MovementState { idle, walk }
    public int maxHealth = 100;
    int currentHealth;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = this.GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
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
        if(m<0 && facing)
        {
            flip();
        }
        else if (m>0 && !facing)
        {
            flip();
        }

        // animation update
        UpdateAnimationUpdate(); 

        // for player picking up item
        if (pickupAllow && Input.GetKeyDown(KeyCode.E))
        {
            Pickup();
        }

        // player health check message
        Debug.Log("Player current health: " + currentHealth);
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

    // knockback
    public float knockback;
    public float knockbackLength;
    public float knockbackCount;
    public bool knockfromRight;
    
    
    // choose item pickup function
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("swordBox") || collision.CompareTag("staffBox") || collision.CompareTag("hammerBox"))
        {
            pickupAllow = true;
        }

    // theFalsePresence
        if (collision.CompareTag("theFalsePresenceDamageBox"))
        {
            theFalsePresence();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("swordBox") || collision.CompareTag("staffBox") || collision.CompareTag("hammerBox"))
        {
            pickupAllow = false;
        }
    }

    // player condition on every enemy
    void theFalsePresence()
    {
        currentHealth -= 3; //TheFalsePresence damage
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Pickup()
    {
        this.sr.enabled = false;
    }

    
    
    
    void Die()
    {
        Debug.Log("Player died");
    }
}

