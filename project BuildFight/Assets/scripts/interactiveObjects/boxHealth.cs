using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boxHealth : MonoBehaviour
{
    [HideInInspector]
    public int currentHealth = 100;

    void Update()
    {
        AdjustCurrentHealth(0);

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
