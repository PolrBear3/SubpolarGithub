using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class snowController : MonoBehaviour
{
    private Rigidbody2D rb;
    Vector2 move;
    [SerializeField] private float moveSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        move = gameObject.transform.position;
        move.x += moveSpeed;
        gameObject.transform.position = move;
    }
}
