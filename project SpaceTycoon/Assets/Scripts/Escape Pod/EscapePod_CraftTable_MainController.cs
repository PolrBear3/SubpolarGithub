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

    private void Update()
    {
        snapCheck();
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

    private void snapCheck()
    {
        if (snapPoint1.activeSelf == false)
        {
            positionButton1.SetActive(false);
        }
        else if (snapPoint1.activeSelf == true)
        {
            positionButton1.SetActive(true);
        }

        if (snapPoint2.activeSelf == false)
        {
            positionButton2.SetActive(false);
        }
        else if (snapPoint2.activeSelf == true)
        {
            positionButton2.SetActive(true);
        }

        if (snapPoint3.activeSelf == false)
        {
            positionButton3.SetActive(false);
        }
        else if (snapPoint3.activeSelf == true)
        {
            positionButton3.SetActive(true);
        }

        if (snapPoint4.activeSelf == false)
        {
            positionButton4.SetActive(false);
        }
        else if (snapPoint4.activeSelf == true)
        {
            positionButton4.SetActive(true);
        }

        if (snapPoint5.activeSelf == false)
        {
            positionButton5.SetActive(false);
        }
        else if (snapPoint5.activeSelf == true)
        {
            positionButton5.SetActive(true);
        }

        if (snapPoint6.activeSelf == false)
        {
            positionButton6.SetActive(false);
        }
        else if (snapPoint6.activeSelf == true)
        {
            positionButton6.SetActive(true);
        }

        if (snapPoint7.activeSelf == false)
        {
            positionButton7.SetActive(false);
        }
        else if (snapPoint7.activeSelf == true)
        {
            positionButton7.SetActive(true);
        }
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
