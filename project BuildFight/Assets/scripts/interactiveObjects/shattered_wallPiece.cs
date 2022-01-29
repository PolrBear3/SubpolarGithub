using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shattered_wallPiece : MonoBehaviour
{
    Rigidbody2D rb;
    float dirX;
    float dirY;
    float torque;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        dirX = Random.Range(-1, 5);
        dirY = Random.Range(-1, 5);
        torque = Random.Range(5, 15);

        rb.AddForce(new Vector2(dirX, dirY), ForceMode2D.Impulse);
        rb.AddTorque(torque, ForceMode2D.Force);

        Destroy(gameObject, 5f);
    }
}
