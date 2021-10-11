using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class theFalsePresenceCondition : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        // theFalsePresence condition
        print("theFalsePresenceHealth: " + currentHealth);
        if (currentHealth <= 0)
        {
            GetComponent<Collider2D>().enabled = false;
            theFalsePresence.anim.SetBool("isDead", true);
            theFalsePresence.moveSpeed = 0f;
            Destroy(transform.parent.gameObject, 6f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // attack by sword
        if (collision.CompareTag("swordDamagePoint"))
        {
            currentHealth -= 10;
            theFalsePresence.anim.SetTrigger("Hurt");
        }
        // attack by hammer
        if (collision.CompareTag("hammerDamagePoint"))
        {
            currentHealth -= 30;
            theFalsePresence.anim.SetTrigger("Hurt");
        }
    }


}
