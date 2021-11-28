using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
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
        if(moveSpeed < maxSpeed)
        {
            moveSpeed += 0.00001f * Time.deltaTime;
        }
        move.x += moveSpeed;
        gameObject.transform.position = move;


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
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        //Slide
        if (Input.GetKeyDown(KeyCode.DownArrow))
        { 
            anim.SetTrigger("slide");
        }
    }

    //Drop
    public Transform dropPoint;
    public GameObject[] presents;
    int randomInt;
    float dropRate = 2f;
    float nextDropTime = 0f;
    void Drop()
    {
        randomInt = Random.Range(0, presents.Length);
        Instantiate(presents[randomInt], dropPoint.position, dropPoint.rotation);
    }
    
    //Jump
    public float jumphight;
    bool isGrounded;
    public Transform groundCheck;
    public LayerMask groundLayer;
    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumphight);
    }

    //GameOver
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("destroyBox"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
            coinText.coinSave();
            scoreManager.highscoreSave();
            //setting current score back to 0
            scoreManager.score = 0.0f;
        }
    }
}
