using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    Vector2 constForward;
    [SerializeField] private float moveSpeed;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    private void Update()
    {
        //Movement
        constForward = gameObject.transform.position;
        constForward.x += moveSpeed;
        gameObject.transform.position = constForward;
        
        //Drop
        if (Time.time > nextDropTime)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Drop();
                nextDropTime = Time.time + 1f / dropRate;
            }
        }

        //Jump
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (isGrounded)
            {
                Jump();
            }
        }
    }

    //Drop
    public Transform dropPoint;
    public GameObject present;
    float dropRate = 2f;
    float nextDropTime = 0f;
    void Drop()
    {
        Instantiate(present, dropPoint.position, dropPoint.rotation);
    }
    
    //Jump
    public float jumphight;
    bool isGrounded;
    public Transform groundCheck;
    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumphight);
    }
}
