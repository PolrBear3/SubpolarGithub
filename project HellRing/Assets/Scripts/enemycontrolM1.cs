using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemycontrolM1 : MonoBehaviour
{
    private Rigidbody2D rb;
    Animator anim;
    public int maxHealth = 100;
    int currentHealth;
    Transform target;
    [SerializeField]
    float agroRange;
    [SerializeField]
    float moveSpeed;
    bool melee = false;

    public float knockback;
    public float knockbackLength;
    public float knockbackCount;
    public bool knockfromRight;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // distance to player
        float distance = Vector2.Distance(transform.position, target.position);

        // agro and chase
        if (distance < agroRange)
        {
            movementChaseKnockback();
            anim.SetBool("Walk", true);
        }
        else
        {
            rb.velocity = new Vector2(0, 0);
            anim.SetBool("Walk", false);
        }

        void movementChaseKnockback()
        {
            if (knockbackCount <= 0)
            {
                if (transform.position.x < target.position.x)
                {
                    // target is on the left so move right 
                    rb.velocity = new Vector2(moveSpeed, 0);
                    transform.localScale = new Vector2(1, 1);
                }
                else if (transform.position.x > target.position.x)
                {
                    // target is on the right so move left
                    rb.velocity = new Vector2(-moveSpeed, 0);
                    transform.localScale = new Vector2(-1, 1);
                }
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
        }
        
        // attack 
        if (melee)
        {
            anim.SetBool("Melee", true);
        }
        else
        {
            anim.SetBool("Melee", false);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            melee = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            melee = false;
        }
    }

    // theFalsePresence condition
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        anim.SetTrigger("Hurt");
        if(currentHealth <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        anim.SetBool("isDead", true);
        moveSpeed = 0f;
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 6f);
    }
}
