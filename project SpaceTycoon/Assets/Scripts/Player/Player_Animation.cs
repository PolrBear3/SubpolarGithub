using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Animation : MonoBehaviour
{
    public Player_MainController playerController;
    Animator anim;

    public RuntimeAnimatorController[] outfitRuntimeAnimators;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        MoveCheck_Animation_Update();
    }

    public void Outfit_RunTimeAnimator_Set()
    {
        anim.runtimeAnimatorController = playerController.playerOutfit.currentOutfit.outfitRuntimeAnimator;
    }
    
    void MoveCheck_Animation_Update()
    {
        if (playerController.playerMovement.isMoving())
        {
            anim.SetBool("isMoving", true);
        }
        else
        {
            anim.SetBool("isMoving", false);
        }
    }

    public void Restart_All_Animation()
    {
        anim.SetBool("isSitting", false);
        anim.SetBool("isSleeping", false);
    }
    public void Set_Sit_Animation()
    {
        anim.SetBool("isSitting", true);
    }
    public void Set_Sleep_Animation()
    {
        anim.SetBool("isSleeping", true);
    }
}
