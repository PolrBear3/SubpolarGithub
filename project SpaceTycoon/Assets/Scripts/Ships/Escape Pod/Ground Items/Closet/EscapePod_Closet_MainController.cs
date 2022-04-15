using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapePod_Closet_MainController : MonoBehaviour
{
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    Animator anim;
    
    [HideInInspector]
    public bool playerDetection;

    public GameObject Icon, iconBoxCollider, mainPanel;
    public Icon icon;

    // innerWear
    public GameObject innerWearSelectButton;
    // spaceSuit
    public GameObject spaceSuitOptionButton, spaceSuitOptionPanel, spaceSuitSelectButton;
    // pajamas
    public GameObject pajamasOptionButton, pajamasOptionPanel, pajamasSelectButton;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("player_hand"))
        {
            playerDetection = true;
            icon.Set_Icon_Position();
            anim.SetBool("playerDetected", true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("player_hand"))
        {
            playerDetection = false;
            icon.Set_Icon_to_Default_Position();
            anim.SetBool("playerDetected", false);
        }
    }
}
