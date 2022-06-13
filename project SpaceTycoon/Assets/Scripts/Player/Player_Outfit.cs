using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Outfit : MonoBehaviour
{
    public Player_MainController playerController;
    public Albert_Outfits currentOutfit;

    public void Update_Player_Outfit(Albert_Outfits closetOutfit)
    {
        currentOutfit = closetOutfit;
        playerController.playerAnimation.Outfit_RunTimeAnimator_Set();
    }
}
