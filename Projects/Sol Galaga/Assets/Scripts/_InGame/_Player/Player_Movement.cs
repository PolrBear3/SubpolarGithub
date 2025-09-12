using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigibody;
    public Rigidbody2D rigibody => _rigibody;

    [Space(20)] 
    [SerializeField] [Range(0, 20)] private float _defaultMoveSpeed;
    
    
    // UnityEngine
    private void FixedUpdate()
    {
        Movement_Update();
    }


    // Main
    private void Movement_Update()
    {
        Vector2 direction = Input_Controller.instance.movementDirection;
        _rigibody.velocity = new Vector2(direction.x * _defaultMoveSpeed, direction.y * _defaultMoveSpeed);
    }
}
