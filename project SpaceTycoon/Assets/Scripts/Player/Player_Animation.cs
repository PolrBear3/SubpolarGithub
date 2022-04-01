using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Animation : MonoBehaviour
{
    public Player_MainController playerController;

    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        MoveCheck_Animation();
        Sit_Sleep_Check_Animation();
        Set_Animation_Accordingto_Outfit();
    }

    void MoveCheck_Animation()
    {
        if (playerController.playerMovement.horizontal > 0 || playerController.playerMovement.horizontal < 0)
        {
            anim.SetBool("isMoving", true);
        }
        else
        {
            anim.SetBool("isMoving", false);
        }
    }

    void Sit_Sleep_Check_Animation()
    {
        if (Player_State.player_isSitting == true)
        {
            anim.SetBool("isSitting", true);
        }
        else
        {
            anim.SetBool("isSitting", false);
        }

        if (Player_State.player_isSleeping == true)
        {
            anim.SetBool("isSleeping", true);
        }
        else
        {
            anim.SetBool("isSleeping", false);
        }
    }

    public RuntimeAnimatorController innerWear, spaceSuit, pajamas;
    void Set_Animation_Accordingto_Outfit()
    {
        if (Player_Outfit.outfitNum == 1)
        {
            anim.runtimeAnimatorController = innerWear as RuntimeAnimatorController;
        }
        if (Player_Outfit.outfitNum == 2)
        {
            anim.runtimeAnimatorController = spaceSuit as RuntimeAnimatorController;
        }
        if (Player_Outfit.outfitNum == 3)
        {
            anim.runtimeAnimatorController = pajamas as RuntimeAnimatorController;
        }
    }
}
