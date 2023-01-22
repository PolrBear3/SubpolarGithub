using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box_RayCast : MonoBehaviour, IDamagable
{
    [SerializeField] private int durability;

    public void Take_Damage(int damageAmount)
    {
        durability -= damageAmount;
        Debug.Log("Box Damaged");
        Break_Check();
    }

    private void Break_Check()
    {
        if (durability <= 0)
        {
            Destroy(gameObject);
        }
    }
}
