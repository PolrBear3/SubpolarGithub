using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shattered_wallPiece : MonoBehaviour
{
    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Scatter_and_Disapear();
    }

    float dirX;
    float dirY;
    float torque;
    void Scatter_and_Disapear()
    {
        dirX = Random.Range(-3, 3);
        dirY = Random.Range(-3, 3);
        torque = Random.Range(5, 15);

        rb.AddForce(new Vector2(dirX, dirY), ForceMode2D.Impulse);
        rb.AddTorque(torque, ForceMode2D.Force);

        Destroy(gameObject, 4f);
    }
}
