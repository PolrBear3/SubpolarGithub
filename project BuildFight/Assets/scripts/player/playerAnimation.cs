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
        Player_Move_Animation();
    }

    void Player_Move_Animation()
    {
        if (playerMovement.horizontal > 0 || playerMovement.horizontal < 0 ||
            playerMovement.vertical > 0 || playerMovement.vertical < 0)
        {
            anim.SetFloat("isMoving", playerMovement.horizontal);
        }
        else
        {
            anim.SetFloat("isMoving", 0);
        }
    }
}
