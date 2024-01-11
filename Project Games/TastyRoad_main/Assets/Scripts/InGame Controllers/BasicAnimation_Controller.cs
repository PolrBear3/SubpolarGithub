using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAnimation_Controller : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out SpriteRenderer spriteRenderer)) { _spriteRenderer = spriteRenderer; }
        if (gameObject.TryGetComponent(out Animator animator)) { _animator = animator; }
    }

    // Sprite Flip Control
    public void Flip_Sprite(bool facingLeft)
    {
        _spriteRenderer.flipX = facingLeft;
    }
    public void Flip_Sprite(float faceDirection)
    {
        if (faceDirection < 0)
        {
            Flip_Sprite(true);
        }
        else if (faceDirection > 0)
        {
            Flip_Sprite(false);
        }
    }

    // Basic Animation Control
    public void Idle_Move(bool isMoving)
    {
        _animator.SetBool("isMoving", isMoving);
    }
}