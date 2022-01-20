using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dropped_objects : MonoBehaviour
{
    private bool scan;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && scan == true && object_controller.hasItem == false)
        {
            Destroy(gameObject, 0.01f);
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
