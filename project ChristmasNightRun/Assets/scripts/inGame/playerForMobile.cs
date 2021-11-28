using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class playerForMobile : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    Vector2 move;
    public float moveSpeed;
    public float maxSpeed;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        //Move
        move = gameObject.transform.position;
        if (moveSpeed < maxSpeed)
        {
            moveSpeed += 0.00001f * Time.deltaTime;
        }
        move.x += moveSpeed;
        gameObject.transform.position = move;

        //for GroundCheck
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    //Drop
    public Transform dropPoint;
    public GameObject[] presents;
    int randomInt;
    float dropRate = 2f;
    float nextDropTime = 0f;
    public void Drop()
    {
        if (Time.time > nextDropTime)
        {
            randomInt = Random.Range(0, presents.Length);
            Instantiate(presents[randomInt], dropPoint.position, dropPoint.rotation);
            nextDropTime = Time.time + 1f / dropRate;
        }
    }

    //Jump
    public float jumphight;
    bool isGrounded;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public void Jump()
    {
        if (isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumphight);
        }
    }

    //Slide
    public void Slide()
    {
        anim.SetTrigger("slide");
    }

    //GameOver
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("destroyBox"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
            coinText.coinSave();
        }
    }
}
