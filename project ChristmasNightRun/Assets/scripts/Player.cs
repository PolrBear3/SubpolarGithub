using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D bc;
    private Vector2 colliderSize;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        colliderSize = bc.size;
    }
    
    private void Update()
    {
        if (Time.time > nextDropTime)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Drop();
                nextDropTime = Time.time + 1f / dropRate;
            }
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (isGrounded)
            {
                Jump();
            }
        }
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        slopeCheck();
    }

    public Transform dropPoint;
    public GameObject present;
    float dropRate = 2f;
    float nextDropTime = 0f;
    void Drop()
    {
        Instantiate(present, dropPoint.position, dropPoint.rotation);
    }
    
    public float jumphight;
    bool isGrounded;
    public Transform groundCheck;
    public LayerMask groundLayer;
    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumphight);
    }

    [SerializeField]
    private float slopeCheckDistance;

    private void slopeCheck()
    {
        Vector2 checkPos = transform.position - new Vector3(0.0f, colliderSize.y / 2);

        slopcheckVertical(checkPos);
    }

    private void slopCheckHorizontal(Vector2 checkPos)
    {

    }
    private void slopcheckVertical(Vector2 checkPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, groundLayer);
        if (hit)
        {
            Debug.DrawRay(hit.point, hit.normal, Color.green);
        }
    }
}
