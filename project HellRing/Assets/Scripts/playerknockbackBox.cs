using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerknockbackBox : MonoBehaviour
{
    public float knockbackPower;
    public float knockbackDuration = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            StartCoroutine(theFalsePresence.TFPinstance.Knockback(knockbackDuration, knockbackPower, this.transform));
        }
    }
}
