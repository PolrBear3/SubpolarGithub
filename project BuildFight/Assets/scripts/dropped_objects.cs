using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dropped_objects : MonoBehaviour
{
    bool scan;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && scan == true)
        {
            Destroy(gameObject, 0.01f);
        }

        if(object_controller.hasItem == true)
        {
            GetComponent<BoxCollider2D>().enabled = false;
        }
        else if(object_controller.hasItem == false)
        {
            GetComponent<BoxCollider2D>().enabled = true;
        }
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
}
