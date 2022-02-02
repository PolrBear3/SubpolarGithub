using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dropped_objects : MonoBehaviour
{
    Rigidbody2D rb;
    
    bool scan;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Object_Scatter();
    }

    private void Update()
    {
        Object_PickedUp();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("hands"))
        {
            scan = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("hands"))
        {
            scan = false;
        }
    }

    float dirY;
    void Object_Scatter()
    {
        dirY = -2;

        rb.AddForce(new Vector2(0, dirY), ForceMode2D.Impulse);
    }

    void Object_PickedUp()
    {
        if (Input.GetKeyDown(KeyCode.E) && scan == true)
        {
            Destroy(gameObject, 0.01f);
        }

        if (object_controller.hasItem == true)
        {
            GetComponent<BoxCollider2D>().enabled = false;
        }
        else if (object_controller.hasItem == false)
        {
            GetComponent<BoxCollider2D>().enabled = true;
        }
    }
}
