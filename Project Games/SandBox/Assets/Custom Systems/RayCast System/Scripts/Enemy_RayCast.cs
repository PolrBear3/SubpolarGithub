using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    void Take_Damage(int damageAmount);
}

public class Enemy_RayCast : MonoBehaviour, IDamagable
{
    [SerializeField] private int health;

    public void Take_Damage(int damageAmount)
    {
        health -= damageAmount;
        Debug.Log("Enemy Damaged");
        Death_Check();
    }

    private void Death_Check()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
