using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class presents5 : MonoBehaviour
{
    float speed = 2f;
    private Rigidbody2D rb;
    float rotSpeed = 3f;

    [SerializeField] private skinManager5 skinManager5;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;

        GetComponent<SpriteRenderer>().sprite = skinManager5.GetSelectedSkin5().sprite5;
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
