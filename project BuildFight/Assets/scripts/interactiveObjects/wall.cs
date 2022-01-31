using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wall : MonoBehaviour
{
    SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        AdjustCurrentHealth(0);

        Wall_Damaged();

        Wall_Destroyed();
    }

    public int currentHealth;
    public void AdjustCurrentHealth(int damage)
    {
        currentHealth += damage;
    }

    void Wall_Damaged()
    {
        if (currentHealth <= 100)
        {
            sr.sprite = broken_wall;
        }
    }

    public Sprite broken_wall;
    public GameObject piece1, piece2, piece3, piece4, piece5, piece6;
    void Wall_Destroyed()
    {
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
}
