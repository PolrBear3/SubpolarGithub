using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike_Movement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rb;
    public Rigidbody2D rb => _rb;

    [Space(20)] 
    [SerializeField] [Range(0, 10)] private float _defaultSpeed;

    
    // MonoBehaviour
    private void FixedUpdate()
    {
        Movement_Update();
    }
    
    
    //
    private void Movement_Update()
    {
        Vector2 direction = Main_InputSystem.instance.movementDirection;
        _rb.velocity = new Vector2(direction.x * _defaultSpeed, direction.y * _defaultSpeed);
    }
}