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
    public snapPoint GsnapPoint1, GsnapPoint2, GsnapPoint3, 
                     GsnapPoint4, GsnapPoint5, GsnapPoint6, GsnapPoint7;

    public GameObject allWallSnapPoints;
    public snapPoint WsnapPoint1, WsnapPoint2, WsnapPoint3,
                     WsnapPoint4, WsnapPoint5, WsnapPoint6, WsnapPoint7;

    public GameObject positionButton1, positionButton2, positionButton3, 
                      positionButton4, positionButton5, positionButton6, positionButton7;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("player_hand"))
        {
            playerDetection = true;
            anim.SetBool("onMenu", true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("player_hand"))
        {
            playerDetection = false;
            anim.SetBool("onMenu", false);
        }
    }
}
