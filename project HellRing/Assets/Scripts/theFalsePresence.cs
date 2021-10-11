using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class theFalsePresence : MonoBehaviour
{
    private Rigidbody2D rb;
    public static Animator anim;
    Transform target;
    public static float TFPagroRange = 6f;
    public static float TFPmoveSpeed = 2f;
    public static bool melee = false;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // distance to player
        float distance = Vector2.Distance(transform.position, target.position);

        // agro and chase
        if (distance < TFPagroRange)
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

    // movement, chase
    void movementChase()
    {
        if (transform.position.x < target.position.x)
        {
            // target is on the left so move right 
            rb.velocity = new Vector2(TFPmoveSpeed, 0);
            transform.localScale = new Vector2(1, 1);
        }
        else if (transform.position.x > target.position.x)
        {
            // target is on the right so move left
            rb.velocity = new Vector2(-TFPmoveSpeed, 0);
            transform.localScale = new Vector2(-1, 1);
        }
    }

    // knockback
    public static theFalsePresence TFPinstance;
    
    private void Awake()
    {
        TFPinstance = this;
    }

    public IEnumerator Knockback(float knockbackDuration, float knockbackPower, Transform obj)
    {
        float timer = 0;

        while (knockbackDuration > timer)
        {
            timer += Time.deltaTime;
            Vector2 direction = (obj.transform.position - this.transform.position).normalized;
            rb.AddForce(-direction * knockbackPower);
        }
        yield return 0;
    }
}
