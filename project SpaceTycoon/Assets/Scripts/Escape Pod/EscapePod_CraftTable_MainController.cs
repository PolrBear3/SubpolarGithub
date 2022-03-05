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
    public GameObject optionsMenu;

    public GameObject allSnapPoints;

    public GameObject snapPoint1, snapPoint2, snapPoint3, 
                      snapPoint4, snapPoint5, snapPoint6, snapPoint7;

    public GameObject positionButton1, positionButton2, positionButton3, 
                      positionButton4, positionButton5, positionButton6, positionButton7;

    public void Restart()
    {
        snapPoint1.SetActive(true);
        snapPoint2.SetActive(true);
        snapPoint3.SetActive(true);
        snapPoint4.SetActive(true);
        snapPoint5.SetActive(true);
        snapPoint6.SetActive(true);
        snapPoint7.SetActive(true);

        positionButton1.SetActive(true);
        positionButton2.SetActive(true);
        positionButton3.SetActive(true);
        positionButton4.SetActive(true);
        positionButton5.SetActive(true);
        positionButton6.SetActive(true);
        positionButton7.SetActive(true);
    }

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
