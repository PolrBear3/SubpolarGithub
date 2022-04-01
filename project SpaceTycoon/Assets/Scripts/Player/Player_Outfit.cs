using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Outfit : MonoBehaviour
{
    private void Start()
    {
        Default_Outfit();
    }

    private void Update()
    {
        Outfit_Num_Set();
    }

    private SpriteRenderer sr;

    public Player_MainController playerController;

    public Albert_Outfits currentOutFit;
    public static int outfitNum;
    
    public static bool isSpaceSuitUnlocked = false, isPajamasUnlocked = false;
    public Albert_Outfits innerWear, spaceSuit, pajamas;

    void Default_Outfit()
    {
        outfitNum = 1;
    }

    void Outfit_Num_Set()
    {
        if (outfitNum == 1)
        {
            currentOutFit = innerWear;
        }
        if (outfitNum == 2)
        {
            currentOutFit = spaceSuit;
        }
        if (outfitNum == 3)
        {
            currentOutFit = pajamas;
        }
    }
}
