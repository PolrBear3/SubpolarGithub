using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    private Game_Controller _gameController;

    [HideInInspector] public Player_Movement playerMovement;
    [HideInInspector] public Player_Animation playerAnimation;
    [HideInInspector] public Player_Interaction playerInteraction;

    // UnityEngine
    private void Awake()
    {
        _gameController = FindObjectOfType<Game_Controller>();

        if (gameObject.TryGetComponent(out Player_Movement playerMovement)) this.playerMovement = playerMovement;
        if (gameObject.TryGetComponent(out Player_Animation playerAnimation)) this.playerAnimation = playerAnimation;
        if (gameObject.TryGetComponent(out Player_Interaction playerInteraction)) this.playerInteraction = playerInteraction;
    }
    private void Start()
    {
        _gameController.Connect_Character(gameObject);
    }
}
