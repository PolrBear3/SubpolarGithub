using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buildingScroller : MonoBehaviour
{
    public BoxCollider2D bc;
    public Rigidbody2D rb;
    private float width;
    private float scrollSpeed = -0.8f;

    void Start()
    {
        bc = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();

        width = bc.size.x;
        bc.enabled = false;

        rb.velocity = new Vector2(scrollSpeed, 0);
    }

    void Update()
    {
        if (transform.position.x < -width)
        {
            Vector2 resetPosition = new Vector2(width * 4f, 0);
            transform.position = (Vector2)transform.position + resetPosition;
        }
    }
}
