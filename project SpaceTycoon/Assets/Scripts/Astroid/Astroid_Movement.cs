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
    
    float currentSpeed = 0f, setSpeed, acceleration = 0.05f;

    void Movement()
    {
        rb.velocity = new Vector2(dirX * currentSpeed, rb.velocity.y);
    }

    void Speed_Control()
    {
        if (SpaceTycoon_Main_GameController.EnginesOn == 0)
        {
            setSpeed = 0f;
        }
        if (SpaceTycoon_Main_GameController.EnginesOn == 1)
        {
            setSpeed = 0.5f;
        }
        if (SpaceTycoon_Main_GameController.EnginesOn == 2)
        {
            setSpeed = 1f;
        }
        if (SpaceTycoon_Main_GameController.EnginesOn == 3)
        {
            setSpeed = 1.5f;
        }

        // low
        if (currentSpeed < setSpeed)
        {
            currentSpeed = currentSpeed + (acceleration * Time.deltaTime);
        }
        // high
        else if (currentSpeed > setSpeed)
        {
            currentSpeed = currentSpeed - (acceleration * Time.deltaTime);
        }
    }
}
