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

    public GameObject Icon;
    public Icon icon;
    public GameObject mainPanel;
    public EscapePod_CraftTable_Panel mainPanelScript;
    public GameObject craftTable_options, chairBed_options, closet_options;

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
            icon.Set_Icon_Position();
            anim.SetBool("onMenu", true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("player_hand"))
        {
            playerDetection = false;
            icon.Set_Icon_to_Default_Position();
            anim.SetBool("onMenu", false);
        }
    }
}
