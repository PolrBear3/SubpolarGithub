using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_RayCast : MonoBehaviour
{
    [SerializeField] private int attackDamage;

    [SerializeField] private Transform boxAttackPoint;
    [SerializeField] private Vector2 boxSize;

    [SerializeField] private Transform circleAttackPoint;
    [SerializeField] private float circleSize;

    private void Update()
    {
        BoxRayCast_Attack_Right();
        CircleRayCast_Attack_Left();
    }

    private void BoxRayCast_Attack_Right()
    {
        if (!Input.GetKeyDown(KeyCode.E)) return;

        Collider2D[] damagables = Physics2D.OverlapBoxAll(boxAttackPoint.position, boxSize, 0);

        for (int i = 0; i < damagables.Length; i++)
        {
            if (!damagables[i].TryGetComponent(out IDamagable damagable)) return;

            damagable.Take_Damage(attackDamage);
        }
    }

    private void CircleRayCast_Attack_Left()
    {
        if (!Input.GetKeyDown(KeyCode.Q)) return;

        Collider2D[] damagables = Physics2D.OverlapCircleAll(circleAttackPoint.position, circleSize);

        for (int i = 0; i < damagables.Length; i++)
        {
            if (!damagables[i].TryGetComponent(out IDamagable damagable)) return;

            damagable.Take_Damage(attackDamage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (boxAttackPoint == null || circleAttackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxAttackPoint.position, boxSize);
        Gizmos.DrawWireSphere(circleAttackPoint.position, circleSize);
    }
}
