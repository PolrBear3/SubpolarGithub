using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Outfit : MonoBehaviour
{
    private void Start()
    {
        Default_Outfit();
    }

    public Player_MainController playerController;

    public Albert_Outfits currentOutFit;

    public Albert_Outfits innerWear, spaceSuit;

    void Default_Outfit()
    {
        currentOutFit = innerWear;
    }
}
