using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hammerPlayer : MonoBehaviour
{
    public SpriteRenderer sr;
    private Animator anim;
    private bool pickupAllow;
    private float m;
    private enum MovementState { hammerPlayerIdle, hammerPlayerWalk }
    public float attackRate = 2f;
    float nextAttackTime = 0f;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // item pickup update
        if (pickupAllow && Input.GetKeyDown(KeyCode.E))
        {
            Pickup();
        }

        // melee attack update and attack cool time
        if(Time.time >= nextAttackTime)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Melee();
                nextAttackTime = Time.time + 3f / attackRate;
            }
        }

        // player movement update
        m = Input.GetAxisRaw("Horizontal");


        // animation update
        updateAnim();
    }
   
    // movement animation
    private void updateAnim()
    {
        MovementState state;

        if (m > 0f)
        {
            state = MovementState.hammerPlayerWalk;
        }
        else if (m < 0f)
        {
            state = MovementState.hammerPlayerWalk;
        }
        else
        {
            state = MovementState.hammerPlayerIdle;
        }
        
        anim.SetInteger("state", (int)state);
    }

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // item pickup function
        if (collision.CompareTag("hammerBox"))
        {
            pickupAllow = true;
        }

        // damaged by theFalsePresence
        if (collision.CompareTag("theFalsePresenceDamageBox"))
        {
            anim.SetTrigger("Hurt");
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("hammerBox"))
        {
            pickupAllow = false;
        }
    }

    private void Pickup()
    {
        this.sr.enabled = true;
        Destroy(GameObject.FindWithTag("swordPlayer"));
        Destroy(GameObject.FindWithTag("staffPlayer"));
    }

    // attack function
    private void Melee()
    {
        anim.SetTrigger("Melee");
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(damagePoint.position, damageRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<enemycontrolM1>().TakeDamage(30); //attack damage 
        }
    }
    public Transform damagePoint;
    public float damageRange;
    public LayerMask enemyLayers;
    private void OnDrawGizmosSelected()
    {
        if (damagePoint == null)
            return;
        Gizmos.DrawWireSphere(damagePoint.position, damageRange);
    }
}
