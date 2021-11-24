using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class presents3 : MonoBehaviour
{
    float speed = 2f;
    private Rigidbody2D rb;
    float rotSpeed = 1.5f;

    [SerializeField] private skinManager3 skinManager3;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;

        GetComponent<SpriteRenderer>().sprite = skinManager3.GetSelectedSkin3().sprite3;
    }

    void Update()
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
