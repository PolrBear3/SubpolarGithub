using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class melee : MonoBehaviour
{
    Animator anim;

    bool scan = false;
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
            if (scan == false)
            {
                scan = true;
                collision.GetComponent<boxHealth>().currentHealth -= damage;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("box"))
        {
            scan = false;
        }
    }
}
