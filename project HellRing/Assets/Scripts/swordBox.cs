using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class swordBox : MonoBehaviour
{
    private Animator anim;
    private bool pickupAllow;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (pickupAllow && Input.GetKeyDown(KeyCode.E))
        {
            Pickup();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            pickupAllow = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            pickupAllow = false;
        }
    }

    private void Pickup()
    {
        Destroy(gameObject, 2f);
        Destroy(GameObject.FindWithTag("staffBox"));
        Destroy(GameObject.FindWithTag("hammerBox"));
        anim.Play("swordBoxClose");
    }
}
