using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blueManSmile : MonoBehaviour
{
    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("present"))
        {
            anim.SetTrigger("smile");
        }
    }
}
