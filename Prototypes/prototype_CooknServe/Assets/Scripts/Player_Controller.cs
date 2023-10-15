using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    private Rigidbody2D _rb;
    public Rigidbody2D rb { get => _rb; set => _rb = value; }

    [SerializeField] private Player_Movement _playerMovement;
    public Player_Movement playerMovement { get => _playerMovement; set => _playerMovement = value; }

    //
    private void Awake()
    {
        Get_AllComponents();
    }

    //
    private void Get_AllComponents()
    {
        if (gameObject.TryGetComponent(out Rigidbody2D rb)) { _rb = rb; }
    }
}
