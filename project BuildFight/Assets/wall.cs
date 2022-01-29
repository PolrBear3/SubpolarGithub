using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wall : MonoBehaviour
{
    [HideInInspector]
    public int currentHealth = 100;
    
    SpriteRenderer sr;
    public Sprite broken_wall;

    public GameObject piece1, piece2, piece3, piece4, piece5, piece6;

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
            Instantiate(piece1, transform.position, Quaternion.identity);
            Instantiate(piece2, transform.position, Quaternion.identity);
            Instantiate(piece3, transform.position, Quaternion.identity);
            Instantiate(piece4, transform.position, Quaternion.identity);
            Instantiate(piece5, transform.position, Quaternion.identity);
            Instantiate(piece6, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }

    public void AdjustCurrentHealth(int damage)
    {
        currentHealth += damage;
    }
}
