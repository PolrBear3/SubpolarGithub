using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    
    float dropRate = 2f;
    float nextDropTime = 0f;
    
    public float jumphight;
    float nextJumpTime = 0f;
    float jumpRate = 2.4f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    private void Update()
    {
        if (Time.time > nextDropTime)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Drop();
                nextDropTime = Time.time + 1f / dropRate;
            }
        }

        if (Time.time > nextJumpTime)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                rb.velocity = new Vector2(rb.velocity.x, jumphight);
                nextJumpTime = Time.time + 1f / jumpRate;
            }
        }
    }

    public Transform dropPoint;
    public GameObject present;
    void Drop()
    {
        Instantiate(present, dropPoint.position, dropPoint.rotation);
    }
}
