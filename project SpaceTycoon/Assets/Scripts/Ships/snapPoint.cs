using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class snapPoint : MonoBehaviour
{
    [HideInInspector]
    public SpriteRenderer sr;

    private bool objectPlaced;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        Object_Place_Check();
    }

    void Object_Place_Check()
    {
        if (objectPlaced == true)
        {
            this.sr.enabled = false;
        }

        if (objectPlaced == false)
        {
            this.sr.enabled = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("object"))
        {
            objectPlaced = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("object"))
        {
            objectPlaced = false;
        }
    }
}
