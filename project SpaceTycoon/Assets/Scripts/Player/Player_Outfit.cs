using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct OutfitUnlock
{
    public Albert_Outfits outfitInfo;
    public bool unlocked;
}

public class Player_Outfit : MonoBehaviour
{
    public Player_MainController playerController;

    public OutfitUnlock[] outfitUnlocks;
    public Albert_Outfits currentOutFit; //connect to runctimeAnimator

    private void Start()
    {

    }

    private void Update()
    {
        Z_for_Innerwear();
        X_for_SpaceSuit();
        C_for_Pajamas();
    }

    public void OutFit_Set_Update (Albert_Outfits outfitInfo)
    {
        for (int i = 0; i < outfitUnlocks.Length; i++)
        {
            if (outfitInfo == outfitUnlocks[i].outfitInfo)
            {
                currentOutFit = outfitUnlocks[i].outfitInfo;
                playerController.playerAnimation.Outfit_RunTimeAnimator_Set();
                break;
            }
        }
    }

    // use these functions as example for closet
    public Albert_Outfits[] outfits;
    
    void Z_for_Innerwear()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            OutFit_Set_Update(outfits[0]);
        }
    }
    void X_for_SpaceSuit()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            OutFit_Set_Update(outfits[1]);
        }
    }
    void C_for_Pajamas()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            OutFit_Set_Update(outfits[2]);
        }
    }
}
