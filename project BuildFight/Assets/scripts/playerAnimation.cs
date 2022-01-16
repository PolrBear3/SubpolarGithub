using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerAnimation : MonoBehaviour
{
    Animator anim;
    
    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if(playerMovement.horizontal == 1 || playerMovement.vertical == 1 ||
           playerMovement.horizontal == -1 || playerMovement.vertical == -1)
        {
            anim.SetBool("isMoving", true);
        }
        else
        {
            anim.SetBool("isMoving", false);
        }
    }
}
