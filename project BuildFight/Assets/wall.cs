using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wall : MonoBehaviour
{
    [HideInInspector]
    public int currentHealth = 100;
    
    SpriteRenderer sr;
    public Sprite broken_wall;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Debug.Log("wallHealth: " + currentHealth);

        AdjustCurrentHealth(0);

        if (currentHealth <= 50)
        {
            sr.sprite = broken_wall;
        }
        
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
