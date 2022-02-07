using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerAnimation : MonoBehaviour
{
    public playerMovement playerMovement;
    
    Animator anim;
    
    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        Player_Animation();
    }

    public Transform player;
    void Player_Animation()
    {
        // mouse position
        Vector3 screenPos = Camera.main.WorldToScreenPoint(player.position);

        if (Input.mousePosition.x < screenPos.x)
        {
            anim.SetTrigger("mouseLeft");
        }
        else
        {
            anim.SetTrigger("mouseRight");
        }

        if (playerMovement.horizontal != 0 || playerMovement.vertical != 0)
        {
            anim.SetBool("isMoving", true);
        }
        else
        {
            anim.SetBool("isMoving", false);
        }
    }
}
