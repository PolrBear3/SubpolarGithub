using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astroid_Movement : MonoBehaviour
{

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); 
    }

    private void Update()
    {
        Speed_Control();
    }

    void FixedUpdate()
    {
        Movement();
    }

    Rigidbody2D rb;

    float dirX = -1;
    
    float speed = 0f;
    float maxSpeed;
    float acceleration = 0.2f;

    void Movement()
    {
        rb.velocity = new Vector2(dirX * speed, rb.velocity.y);
    }
    
    void Speed_Control()
    {
        if (SpaceTycoon_Main_GameController.Shipspeed0 == true)
        {
            maxSpeed = 0f;
            
            // low
            if (speed > maxSpeed)
            {
                speed = speed - (acceleration * Time.deltaTime);
            }
        }
        else if (SpaceTycoon_Main_GameController.Shipspeed1 == true)
        {
            maxSpeed = 1f;
            
            // low
            if (speed < maxSpeed)
            {
                speed = speed + (acceleration * Time.deltaTime);
            }

            // high
            if (speed > maxSpeed)
            {
                speed = speed - (acceleration * Time.deltaTime);
            }
        }
        else if (SpaceTycoon_Main_GameController.Shipspeed2 == true)
        {
            maxSpeed = 2f;

            // low
            if (speed < maxSpeed)
            {
                speed = speed + (acceleration * Time.deltaTime);
            }

            // high
            if (speed > maxSpeed)
            {
                speed = speed - (acceleration * Time.deltaTime);
            }
        }
        else if (SpaceTycoon_Main_GameController.Shipspeed3 == true)
        {
            maxSpeed = 3f;

            // low
            if (speed < maxSpeed)
            {
                speed = speed + (acceleration * Time.deltaTime);
            }

            // high
            if (speed > maxSpeed)
            {
                speed = speed - (acceleration * Time.deltaTime);
            }
        }
    }
}
