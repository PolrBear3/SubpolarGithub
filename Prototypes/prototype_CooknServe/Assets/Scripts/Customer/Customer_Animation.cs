using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer_Animation : MonoBehaviour
{
    private SpriteRenderer _sr;
    private Animator _anim;

    private Customer_Controller _customerController;

    [Header("Data")]
    public float alphaTime;

    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out SpriteRenderer sr)) { _sr = sr; }
        if (gameObject.TryGetComponent(out Animator anim)) { _anim = anim; }
        if (gameObject.TryGetComponent(out Customer_Controller customerController)) { _customerController = customerController; }
    }
    private void Update()
    {
        Walk_Animation();
    }

    // Custom
    public void Spawn_Effect()
    {
        Color spriteColor = _sr.color;
        spriteColor.a = 0f;
        _sr.color = spriteColor;

        LeanTween.alpha(gameObject, 1f, alphaTime);
    }
    public void Leave_Effect()
    {
        LeanTween.alpha(gameObject, 0f, alphaTime);
    }

    private void Walk_Animation()
    {
        if (_customerController.customerMovement.Is_NextPosition())
        {
            _anim.SetBool("isMoving", false);
            return;
        }
        _anim.SetBool("isMoving", true);
    }
}