using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class barrierTB : MonoBehaviour
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
    }

    public int currentHealth;
    void AdjustCurrentHealth(int damage)
    {
        currentHealth += damage;
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
}
