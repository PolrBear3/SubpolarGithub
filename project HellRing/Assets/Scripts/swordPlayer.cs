using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class swordPlayer : MonoBehaviour
{
    public SpriteRenderer sr;
    private Animator anim;
    private bool pickupAllow;
    private float m;
    private enum MovementState { swordPlayerIdle, swordPlayerWalk }
    public float attackRate = 2f;
    float nextAttackTime = 0f;

    private void Start()
    {
        sr = this.GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        // item pickup update
        if (pickupAllow && Input.GetKeyDown(KeyCode.E))
        {
            Pickup();
        }

        // melee attack update and attack cool time
        if (Time.time >= nextAttackTime)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Melee();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
     
        // player movement update
        m = Input.GetAxisRaw("Horizontal");

        // animation update
        updateAnim();
    }

    // Movement animation
    private void updateAnim()
    {
        MovementState state;
        
        if (m > 0f)
        {
            state = MovementState.swordPlayerWalk;
        }    
        else if (m < 0f)
        {
            state = MovementState.swordPlayerWalk;
        }
        else
        {
            state = MovementState.swordPlayerIdle;
        }
        
        anim.SetInteger("state", (int)state);
    }
    
    // Item Pickup function
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("swordBox"))
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
        if (collision.CompareTag("swordBox"))
        {
            pickupAllow = false;
        }
    }

    private void Pickup()
    {
        this.sr.enabled = true;
        Destroy(GameObject.FindWithTag("staffPlayer"));
        Destroy(GameObject.FindWithTag("hammerPlayer"));
    }

    // Attack function
    private void Melee()
    {
        anim.SetTrigger("Melee");
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(damagePoint.position, damageRange, enemyLayers);
        foreach(Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<theFalsepresenceController>().TakeDamage(10); //attack damage 
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
