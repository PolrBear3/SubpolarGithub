using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class staffPlayer : MonoBehaviour
{
    public SpriteRenderer sr;
    public Animator anim;
    private bool pickupAllow;
    private float m;
    private enum MovementState { staffPlayerIdle, staffPlayerWalk }
      
    void Start()
    {
        sr = this.GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // item pickup update
        if (pickupAllow && Input.GetKeyDown(KeyCode.E))
        {
            Pickup();
        }

        // melee update
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // anim.SetTrigger("Melee");
            {
                Melee();
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
            state = MovementState.staffPlayerWalk;
        }
        else if (m < 0f)
        {
            state = MovementState.staffPlayerWalk;
        }
        else
        {
            state = MovementState.staffPlayerIdle;
        }
       
        anim.SetInteger("state", (int)state);
    }
       
        
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // item pickup  
        if (collision.CompareTag("staffBox"))
        {
             pickupAllow = true;
        }

        // damaged by theFalsePresence
        if (collision.CompareTag("enemyknockbackBox"))
        {
            anim.SetTrigger("Hurt");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("staffBox"))
        {
            pickupAllow = false;
        }
    }

    // pickup
    private void Pickup()
    {
        this.sr.enabled = true;
        Destroy(GameObject.FindWithTag("swordPlayer"));
        Destroy(GameObject.FindWithTag("hammerPlayer"));
    }

    // melee
    public Transform shootPoint;
    public GameObject staffParticle;
    
    void Melee()
    {
        Instantiate(staffParticle, shootPoint.position, shootPoint.rotation);
    }
}

