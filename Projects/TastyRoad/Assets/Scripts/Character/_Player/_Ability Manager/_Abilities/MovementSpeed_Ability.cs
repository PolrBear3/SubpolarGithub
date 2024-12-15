using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementSpeed_Ability : Ability_Behaviour, IAbility
{
    [Header("")]
    [SerializeField][Range(0, 1)] private float _increaseStep;


    // IAbility
    public void Activate()
    {
        Player_Movement movement = manager.player.movement;

        movement.Update_MoveSpeed(_increaseStep);
    }
}