using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAnimation_Controller : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    public SpriteRenderer spriteRenderer => _spriteRenderer;

    private Animator _animator;

    [SerializeField] private AnimatorOverrideController _animOverride;
    [SerializeField] private string _defaultAnimation;


    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out SpriteRenderer spriteRenderer)) { _spriteRenderer = spriteRenderer; }
        _animator = gameObject.GetComponent<Animator>();
    }

    private void Start()
    {
        Set_OverrideController();
        Set_DefaultAnimation();
    }

    private void OnEnable()
    {
        _animator.enabled = true;
    }

    private void OnDisable()
    {
        _animator.enabled = false;
    }


    // Custon Animation Play
    private void Set_OverrideController()
    {
        if (_animOverride == null) return;

        _animator.runtimeAnimatorController = _animOverride;
    }
    public void Set_OverrideController(AnimatorOverrideController setOverrider)
    {
        _animOverride = setOverrider;

        Set_OverrideController();
    }

    private void Set_DefaultAnimation()
    {
        if (_defaultAnimation == null) return;

        _animator.Play(_defaultAnimation);
    }

    public void Play_Animation(string animationName)
    {
        _animator.Play(animationName);
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
    /// <summary>
    /// Flips to the direction of target object
    /// </summary>
    public void Flip_Sprite(GameObject targetObject)
    {
        if (transform.position.x > targetObject.transform.position.x)
        {
            Flip_Sprite(true);
        }
        else if (transform.position.x < targetObject.transform.position.x)
        {
            Flip_Sprite(false);
        }
    }


    // Movement Animation Control
    public void Idle_Move(bool isMoving)
    {
        _animator.SetBool("isMoving", isMoving);
    }
}