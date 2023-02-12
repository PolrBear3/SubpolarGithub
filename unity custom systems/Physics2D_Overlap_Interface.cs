using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// interface controller
public interface Idamagable
{
    void Take_Damage(int damageAmount);
}

// player
public class player : MonoBehaviour
{
    // player stats
    [SerializeField] private int attackDamage;
    
    // box
    [SerializeField] private Transform boxCenterPoint;
    [SerializeField] private Vector2 boxSize;
    
    // circle
    [SerializeField] private Transform circleCenterPoint;
    [SerializeField] private float circleSize;
    
    void Box_Detection_Attack()
    {
        Collider2D[] detections = Physics2D.OverlapBoxAll(boxCenterPoint.position, boxSize, 0);
        
        for (int i = 0; i < detections.Length; i++)
        {
            if (!detections[i].TryGet.TryGetComponent(out IDamagable damagable)) return;
            
            // damage detected damagable object 
            damagable.Take_Damage(attackDamage);
        }
    }
    void Circle_Detection_Attack()
    {
        Collider2D[] detections = Physics2D.OverlapCircleAll(circleCenterPoint.position, circleSize);
        
        for (int i = 0; i < detections.Length; i++)
        {
            if (!detections[i].TryGet.TryGetComponent(out IDamagable damagable)) return;
            
            // damage detected damagable object 
            damagable.Take_Damage(attackDamage);
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        // color red
        Gizmos.color = Color.red;
        
        if (boxCenterPoint == null || circleCenterPoint == null) return;  
        
        // box
        Gizmos.DrawWireCube(boxCenterPoint.position, boxSize);
        
        // circle
        Gizmos.DrawWireSphere(circleCenterPoint.position, circleSize);
    }
}

// enemy
public class enemy : MonoBehaviour, IDamagable
{
    BoxCollider2D boxCollider;
    int health;
    
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }
    
    public void Take_Damage(int damageAmount)
    {
        health -= damageAmount;
        Debug.Log("Enemy has been hit!");
    }
}