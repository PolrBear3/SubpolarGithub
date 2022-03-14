using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Icon_Point : MonoBehaviour
{
    [HideInInspector]
    public bool iconDetection;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("icon"))
        {
            iconDetection = true;
        }
    }
}
