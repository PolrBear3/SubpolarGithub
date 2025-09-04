using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAnimation_Controller : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    public SpriteRenderer spriteRenderer => _spriteRenderer;

    private Animator _animator;
    public Animator animator => _animator;


    [Space(20)]
    [SerializeField] private AnimatorOverrideController _animOverride;
    public AnimatorOverrideController animOverride => _animOverride;
    
    [SerializeField] private string _defaultAnimation;


    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out SpriteRenderer spriteRenderer)) { _spriteRenderer = spriteRenderer; }

        if (_animator != null) return;
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
        if (setOverrider == null) return;

        _animOverride = setOverrider;

        Set_OverrideController();
    }

    private void Set_DefaultAnimation()
    {
        if (_animOverride == null) return;
        if (_defaultAnimation == null) return;

        _animator.Play(_defaultAnimation);
    }

    public void Play_Animation(string animationName)
    {
        _animator.Play(animationName, -1, 0f);
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

    public void Flip_Sprite(Vector2 faceDirection)
    {
        Flip_Sprite(faceDirection.x);
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
        if (_animator == null) return;
        if (_animator.runtimeAnimatorController == null) return;

        _animator.SetBool("isMoving", isMoving);
    }
}