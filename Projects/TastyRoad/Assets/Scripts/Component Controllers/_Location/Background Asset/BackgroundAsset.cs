using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundAsset : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    public SpriteRenderer spriteRenderer => _spriteRenderer;
    
    
    private BackgroundAsset_Controller _controller;

    private float _speed;
    private bool _leftDirection;
    
    
    // UnityEngine
    private void Update()
    {
        if (_speed <= 0) return;
        
        Movement_Update();
        Destroy_Update();
    }
    
    
    // Data Control
    public void Set_Controller(BackgroundAsset_Controller controller)
    {
        _controller = controller;
    }

    
    // Movement
    public void Set_Movement(float speed, bool leftDirection)
    {
        _speed = speed;
        _leftDirection = leftDirection;

        if (_leftDirection == false) return;
        _spriteRenderer.flipX = true;
    }

    private void Movement_Update()
    {
        float direction = _leftDirection ? -1f : 1f;
        transform.Translate(Vector2.right * direction * _speed * Time.deltaTime);
    }
    
    
    // Destroy
    private void Destroy_Update()
    {
        Vector2 spawnRange = _controller.spawnRange;

        if (transform.position.x < spawnRange.x || transform.position.x > spawnRange.y)
        {
            _controller.spawnAssets.Remove(this);
            Destroy(gameObject);
        }
    }
}