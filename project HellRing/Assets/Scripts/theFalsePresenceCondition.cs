using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class theFalsePresenceCondition : MonoBehaviour
{
    public static int theFalsePresenceMaxHealth = 100;
    public static int theFalsePresencecurrentHealth;

    void Start()
    {
        theFalsePresencecurrentHealth = theFalsePresenceMaxHealth;
    }

    void Update()
    {
        // theFalsePresence condition
        print("theFalsePresenceHealth: " + theFalsePresencecurrentHealth);
        if (theFalsePresencecurrentHealth <= 0)
        {
            GetComponent<Collider2D>().enabled = false;
            theFalsePresence.theFalsePresenceAnim.SetBool("isDead", true);
            theFalsePresence.TFPmoveSpeed = 0f;
            Destroy(transform.parent.gameObject, 6f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // attack by sword
        if (collision.CompareTag("swordDamagePoint"))
        {
            theFalsePresencecurrentHealth -= 10;
            theFalsePresence.theFalsePresenceAnim.SetTrigger("Hurt");
        }
        // attack by hammer
        if (collision.CompareTag("hammerDamagePoint"))
        {
            theFalsePresencecurrentHealth -= 30;
            theFalsePresence.theFalsePresenceAnim.SetTrigger("Hurt");
        }
    }


}
