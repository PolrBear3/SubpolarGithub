using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapePod_ChairBed_MainController : MonoBehaviour
{
    [HideInInspector]
    public bool playerDetection;

    public GameObject icon;
    public GameObject ChairBedPanel;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerDetection = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerDetection = false;
        }
    }
}
