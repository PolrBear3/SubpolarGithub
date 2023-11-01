using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    [HideInInspector] public Player_Movement playerMovement;
    [HideInInspector] public Player_Animation playerAnimation;
    [HideInInspector] public Player_Interaction playerInteraction;

    //
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Player_Movement playerMovement)) this.playerMovement = playerMovement;
        if (gameObject.TryGetComponent(out Player_Animation playerAnimation)) this.playerAnimation = playerAnimation;
        if (gameObject.TryGetComponent(out Player_Interaction playerInteraction)) this.playerInteraction = playerInteraction;
    }
}
