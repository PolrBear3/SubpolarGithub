using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class presents : MonoBehaviour
{
    float speed = 2f;
    private Rigidbody2D rb;
    float rotSpeed = 3f;

    [SerializeField] private skinManager skinManager;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;

        GetComponent<SpriteRenderer>().sprite = skinManager.GetSelectedSkin().sprite;
    }

    void FixedUpdate()
    {
        transform.Rotate(new Vector3(0, 0, -rotSpeed));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("target") || collision.CompareTag("destroyBox"))
        {
            Destroy(gameObject);
        }
    }
}
