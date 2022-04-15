using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapePod_ChairBed_MainController : MonoBehaviour
{
    [HideInInspector]
    public bool playerDetection;
    [HideInInspector]
    public GameObject player;

    [HideInInspector]
    public bool chairMode = true;

    public GameObject Icon;
    public Icon icon;
    public GameObject iconBoxCollider;
    public GameObject mainPanel;

    public GameObject rotateRightButton;
    public GameObject roateLeftButton;
    public GameObject chairModeButton;
    public GameObject bedModeButton;

    public GameObject sleepButton;
    public GameObject sitButton;

    public GameObject sitPosition;
    public GameObject sleepPosition;

    [HideInInspector]
    public SpriteRenderer sr;
    public Sprite bed;
    public Sprite chair;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("player_hand"))
        {
            playerDetection = true;
            icon.Set_Icon_Position();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("player_hand"))
        {
            playerDetection = false;
            icon.Set_Icon_to_Default_Position();
        }
    }
}
