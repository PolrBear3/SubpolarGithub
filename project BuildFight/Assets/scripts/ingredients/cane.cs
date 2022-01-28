using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cane : MonoBehaviour
{
    Animator anim;

    bool isHit;
    public int damage;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            anim.SetTrigger("cane_Melee");
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
