using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astroid_Movement : MonoBehaviour
{
    public EscapePod_MainController escapePod_controller;

    Rigidbody2D rb;

    float dirX = -1;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); 
    }

    void FixedUpdate()
    {
        Speed_Control();
    }

    void Speed_Control()
    {
        if (escapePod_controller.speed0 == true)
        {
            Speed0();
        }
        else if (escapePod_controller.speed1 == true)
        {
            Speed1();
        }
        else if (escapePod_controller.speed2 == true)
        {
            Speed2();
        }
        else if (escapePod_controller.speed3 == true)
        {
            Speed3();
        }
    }

    float speed0 = 0f;
    void Speed0()
    {
        rb.velocity = new Vector2(dirX * speed0, rb.velocity.y);
    }

    float speed1 = 1f;
    void Speed1()
    {
        rb.velocity = new Vector2(dirX * speed1, rb.velocity.y);
    }

    float speed2 = 2f;
    void Speed2()
    {
        rb.velocity = new Vector2(dirX * speed2, rb.velocity.y);
    }

    float speed3 = 3f;
    void Speed3()
    {
        rb.velocity = new Vector2(dirX * speed3, rb.velocity.y);
    }
}
