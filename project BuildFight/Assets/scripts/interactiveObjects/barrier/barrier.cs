using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class barrier : MonoBehaviour
{
    SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Getting_Damaged_Sprite_Change();
        AdjustCurrentHealth(0);
        Spread_Barrier_Pieces();
        Health_MinMax();
    }

    public int currentHealth;
    void AdjustCurrentHealth(int damage)
    {
        currentHealth += damage;
    }

    void Health_MinMax()
    {
        if (currentHealth > 100)
        {
            currentHealth = 100;
        }

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
    }

    public Sprite unDamaged, damaged1, damaged2, damaged3, fullyDamaged;
    void Getting_Damaged_Sprite_Change()
    {
        if (currentHealth >= 80)
        {
            sr.sprite = unDamaged;
        }

        if (currentHealth < 80 && currentHealth >= 60)
        {
            sr.sprite = damaged1;
        }

        if (currentHealth < 60 && currentHealth >= 40)
        {
            sr.sprite = damaged2;
        }

        if (currentHealth < 40 && currentHealth >= 1)
        {
            sr.sprite = damaged3;
        }

        if (currentHealth == 0)
        {
            sr.sprite = fullyDamaged;
        }
    }

    [HideInInspector]
    public int spreadPiecesCycle = 4;
    public GameObject barrierPiece1, barrierPiece2, barrierPiece3, barrierPiece4, barrierPiece5;
    void Spread_Barrier_Pieces()
    {
        if (spreadPiecesCycle == 0 && currentHealth > 0)
        {    
            Instantiate(barrierPiece1, transform.position, Quaternion.identity);
            Instantiate(barrierPiece2, transform.position, Quaternion.identity);
            Instantiate(barrierPiece3, transform.position, Quaternion.identity);
            Instantiate(barrierPiece4, transform.position, Quaternion.identity);
            Instantiate(barrierPiece5, transform.position, Quaternion.identity);
        }
        if (spreadPiecesCycle == 0)
        {
            spreadPiecesCycle = 4;
        }
    }
}
