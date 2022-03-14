using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapePod_CraftTable_MainController : MonoBehaviour
{
    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    [HideInInspector]
    public bool playerDetection;

    public GameObject icon;
    public GameObject mainPanel;
    public GameObject craftTable_options, chairBed_options;

    public GameObject allGroundSnapPoints;
    public snapPoint snapPoint1, snapPoint2, snapPoint3, 
                     snapPoint4, snapPoint5, snapPoint6, snapPoint7;

    public GameObject positionButton1, positionButton2, positionButton3, 
                      positionButton4, positionButton5, positionButton6, positionButton7;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerDetection = true;
            anim.SetBool("onMenu", true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerDetection = false;
            anim.SetBool("onMenu", false);
        }
    }
}
