using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class snowController : MonoBehaviour
{
    private Rigidbody2D rb;
    Vector2 move;
    public float moveSpeed;
    public float maxSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        move = gameObject.transform.position;
        if (moveSpeed < maxSpeed)
        {
            moveSpeed += 0.00001f * Time.deltaTime;
        }
        move.x += moveSpeed;
        gameObject.transform.position = move;
    }
}
