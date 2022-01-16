using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    public static float horizontal;
    public static float vertical;
    public float speed;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        // single horizontal and vertical movement
        rb.velocity = new Vector2(horizontal * speed, vertical * speed);
        
        // top-right movement
        if (horizontal >= 1 && vertical >= 1)  
        {
            rb.velocity = new Vector2(horizontal * speed, vertical * speed) / 1.3f;
        }
        
        // top-left movement
        if (horizontal <= -1 && vertical >= 1)  
        {
            rb.velocity = new Vector2(horizontal * speed, vertical * speed) / 1.3f;
        }
        
        // bottom-right movement
        if (horizontal >= 1 && vertical <= -1)  
        {
            rb.velocity = new Vector2(horizontal * speed, vertical * speed) / 1.3f;
        }
        
        // bottom-left movement
        if (horizontal <= -1 && vertical <= -1)  
        {
            rb.velocity = new Vector2(horizontal * speed, vertical * speed) / 1.3f;
        }
    }
}
