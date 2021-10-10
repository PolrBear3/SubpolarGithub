using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class theFalsePresence : MonoBehaviour
{
    private Rigidbody2D rb;
    Animator anim;
    public int theFalsePresenceMaxHealth = 100;
    public int theFalsePresencecurrentHealth;
    Transform target;
    [SerializeField]
    float agroRange;
    [SerializeField]
    float moveSpeed;
    public static bool melee = false;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        theFalsePresencecurrentHealth = theFalsePresenceMaxHealth;
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
            movementChase();
            anim.SetBool("Walk", true);
        }
        else
        {
            rb.velocity = new Vector2(0, 0);
            anim.SetBool("Walk", false);
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
        // attack by sword
        if (collision.CompareTag("swordDamagePoint"))
        {
            theFalsePresencecurrentHealth -= 10;
            anim.SetTrigger("Hurt");
        }
        // attack by hammer
        if (collision.CompareTag("hammerDamagePoint"))
        {
            theFalsePresencecurrentHealth -= 30;
            anim.SetTrigger("Hurt");
        }
    }

    void movementChase()
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

    // theFalsePresence condition
    void die()
    {
        if(theFalsePresencecurrentHealth <= 0)
        {
            anim.SetBool("isDead", true);
            moveSpeed = 0f;
            GetComponent<Collider2D>().enabled = false;
            Destroy(gameObject, 6f);
        }
    }
}
