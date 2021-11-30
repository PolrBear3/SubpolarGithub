using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scoreFormatBox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("player"))
        {
            scoreManager.score = 0.0f;
        }
    }
}
