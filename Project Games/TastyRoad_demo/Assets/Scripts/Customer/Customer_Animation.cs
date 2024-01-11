using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer_Animation : MonoBehaviour
{
    private Animator _anim;

    private Customer_Controller _customerController;

    [Header("Data")]
    public float alphaTime;

    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Animator anim)) { _anim = anim; }
        if (gameObject.TryGetComponent(out Customer_Controller customerController)) { _customerController = customerController; }
    }
    private void Update()
    {
        Walk_Animation();
    }

    // Custom
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