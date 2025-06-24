using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike_Movement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rb;
    public Rigidbody2D rb => _rb;
    
    [Space(20)]
    [SerializeField] private Spike _controller;

    [Space(20)] 
    [SerializeField][Range(0, 10)] private float _defaultSpeed;
    [SerializeField][Range(0, 1000)] private float _rotationSpeed;
    
    [Space(10)]
    [SerializeField][Range(0, 10)] private float _knockBackDistance;
    [SerializeField][Range(0, 10)] private float _stunDuration;


    private bool _movementToggle;
    
    private bool _isMoving;
    public bool isMoving => _isMoving;

    
    // MonoBehaviour
    private void Start()
    {
        Toggle_Movement(true);
        
        // subscription
        _controller.OnDamage += Damage_KnockBack;
    }

    private void OnDestroy()
    {
        // subscription
        _controller.OnDamage -= Damage_KnockBack;
    }

    private void FixedUpdate()
    {
        Movement_Update();
    }

    private void Update()
    {
        Rotation_Update();
    }


    // Movement
    public void Toggle_Movement(bool toggle)
    {
        _movementToggle = toggle;
    }
    
    
    private void Movement_Update()
    {
        if (_movementToggle == false)
        {
            _rb.velocity = Vector2.zero;
            return;
        }
        
        Vector2 direction = Main_InputSystem.instance.movementDirection;
        
        _isMoving = direction.x != 0 || direction.y != 0;
        _rb.velocity = new Vector2(direction.x * _defaultSpeed, direction.y * _defaultSpeed);
    }
    
    private void Rotation_Update()
    {
        if (_isMoving == false) return;
        Vector2 direction = _rb.velocity;

        if (direction.sqrMagnitude < 0.01f) return;
        Vector2 moveDir = direction.normalized;
        
        if (direction.sqrMagnitude > 0.01f)
        {
            // Get angle in degrees from velocity vector
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Adjust for upward-facing sprite (if top = +Y)
            targetAngle -= 90f;

            // Smooth rotation
            float currentAngle = transform.eulerAngles.z;
            float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, _rotationSpeed * Time.fixedDeltaTime / 360f);
        
            transform.rotation = Quaternion.Euler(0, 0, newAngle);
        }
    }
    
    
    private void Damage_KnockBack()
    {
        StartCoroutine(KnockBackByPosition());
    }
    private IEnumerator KnockBackByPosition()
    {
        Toggle_Movement(false);

        Vector2 initialPos = transform.position;
        Vector2 knockBackPos = initialPos - (Vector2)transform.up * _knockBackDistance;

        float elapsed = 0f;

        while (elapsed < _stunDuration)
        {
            float t = elapsed / _stunDuration;
            transform.position = Vector2.Lerp(initialPos, knockBackPos, t);
            
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = knockBackPos;
        Toggle_Movement(true);
    }
}