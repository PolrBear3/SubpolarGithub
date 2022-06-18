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
        MoveCheck_Animation();
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

    public void Outfit_RunTimeAnimator_Set()
    {
        anim.runtimeAnimatorController = playerController.playerOutfit.currentOutfit.outfitRuntimeAnimator;
    }
}
