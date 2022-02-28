using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astroid_Movement : MonoBehaviour
{
    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); 
    }

    float dirX;
    float dirY;
    float torque;

    void Start()
    {
        Scatter();
    }

    void Scatter()
    {
        dirX = Random.Range(-1, -5);    // adjust range to faster nums when the ship is going faster
        dirY = Random.Range(0, 2);
        torque = Random.Range(5, 15);

        rb.AddForce(new Vector2(dirX, dirY), ForceMode2D.Impulse);
        rb.AddTorque(torque, ForceMode2D.Force);
    }
}
