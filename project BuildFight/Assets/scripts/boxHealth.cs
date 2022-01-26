using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boxHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth = 100;

    void Update()
    {
        AdjustCurrentHealth(0);
        Debug.Log("boxHealth: " + currentHealth);

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void AdjustCurrentHealth(int damage)
    {
        currentHealth += damage;
    }
}
