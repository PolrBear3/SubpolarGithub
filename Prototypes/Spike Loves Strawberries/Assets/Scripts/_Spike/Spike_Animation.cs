using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike_Animation : MonoBehaviour
{
    [Space(20)]
    [SerializeField] private SpriteRenderer _sr;
    public SpriteRenderer sr => _sr;
    
    [SerializeField] private Animator _animator;
    public Animator animator => _animator;
    
    [Space(20)]
    [SerializeField] private Spike _controller;
    
    
    // MonoBehaviour
    private void Update()
    {
        Animation_Update();
    }
    
    
    // Animation
    private void Animation_Update()
    {
        if (_controller.movement.isMoving == false)
        {
            _animator.speed = 0;
            return;
        }
        _animator.speed = 1;
    }
}