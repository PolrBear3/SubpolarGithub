using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    [SerializeField] private Player_Movement _playerMovement;
    public Player_Movement playerMovement { get => _playerMovement; set => _playerMovement = value; }

    [SerializeField] private Player_Animation _playerAnimation;
    public Player_Animation playerAnimation { get => _playerAnimation; set => _playerAnimation = value; }

    //
}
