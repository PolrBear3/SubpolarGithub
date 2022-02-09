using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shttered_barrierPiece : MonoBehaviour
{
    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Scatter_toRight();
        Scatter_toLeft();
        Scatter_toUp();
        Scatter_toBottom();
        Pieces_disapear();
    }

    float dirX;
    float dirY;
    float torque;

    public bool leftBarrier;
    public bool rightBarrier;
    public bool bottomBarrier;
    public bool topBarrier;
    
    void Scatter_toRight()
    {
        if (leftBarrier == true)
        {
            dirX = Random.Range(2, 4);
            dirY = Random.Range(-2, 4);
            torque = Random.Range(5, 15);

            rb.AddForce(new Vector2(dirX, dirY), ForceMode2D.Impulse);
            rb.AddTorque(torque, ForceMode2D.Force);
        }
    }

    void Scatter_toLeft()
    {
        if (rightBarrier == true)
        {
            dirX = Random.Range(-2, -4);
            dirY = Random.Range(-2, 4);
            torque = Random.Range(5, 15);

            rb.AddForce(new Vector2(dirX, dirY), ForceMode2D.Impulse);
            rb.AddTorque(torque, ForceMode2D.Force);
        }
    }

    void Scatter_toUp()
    {
        if (bottomBarrier == true)
        {
            dirX = Random.Range(-2, 4);
            dirY = Random.Range(2, 4);
            torque = Random.Range(5, 15);

            rb.AddForce(new Vector2(dirX, dirY), ForceMode2D.Impulse);
            rb.AddTorque(torque, ForceMode2D.Force);
        }
    }

    void Scatter_toBottom()
    {
        if (topBarrier == true)
        {
            dirX = Random.Range(-2, 4);
            dirY = Random.Range(-2, -4);
            torque = Random.Range(5, 15);

            rb.AddForce(new Vector2(dirX, dirY), ForceMode2D.Impulse);
            rb.AddTorque(torque, ForceMode2D.Force);
        }
    }

    void Pieces_disapear()
    {
        Destroy(gameObject, 2f);
    }
}
