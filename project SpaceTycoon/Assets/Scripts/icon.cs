using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class icon : MonoBehaviour
{
    private bool otherIconDetect;

    private void Update()
    {
        Icon_Position_Set();
    }

    private void Icon_Position_Set()
    {
        if (otherIconDetect == false)
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-450, 120);
        }
        else if (otherIconDetect == true)
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-450, 60);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("icon"))
        {
            otherIconDetect = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("icon"))
        {
            otherIconDetect = false;
        }
    }
}
