using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplePickup_Ability : Ability_Behaviour, IAbility
{
    [Header("")]
    [SerializeField][Range(0, 100)] private int _increaseValue;


    // IAbility
    public void Activate()
    {
        FoodData_Controller playerIcon = manager.player.foodIcon;
        int currentMax = playerIcon.maxDataCount;

        playerIcon.SetMax_SubDataCount(currentMax + _increaseValue);
    }
}
