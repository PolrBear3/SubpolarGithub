using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class melee : MonoBehaviour
{
    Animator anim;

    bool isHit = false;
    public int damage;
    
    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            anim.SetTrigger("melee");
        }

        if (Input.GetMouseButtonDown(1))
        {
            anim.SetTrigger("melee2");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("box"))
        {
            if (isHit == false)
            {
                isHit = true;
                collision.GetComponent<boxHealth>().currentHealth -= damage;
            }
        }
        if (collision.CompareTag("wall"))
        {
            if (isHit == false)
            {
                isHit = true;
                collision.GetComponent<wall>().currentHealth -= damage;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("box"))
        {
            isHit = false;
        }
        if (collision.CompareTag("wall"))
        {
            isHit = false;
        }
    }
}