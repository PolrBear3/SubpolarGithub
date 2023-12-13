using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer_Animation : MonoBehaviour
{
    private Animator _anim;

    private Customer_Controller _customerController;

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
    public void Spawn_Effect()
    {
        LeanTween.alpha(gameObject, 0f, 0f);
        LeanTween.alpha(gameObject, 1f, 2f);
    }

    private void Walk_Animation()
    {
        if (!_customerController.customerMovement.Is_RoamActive())
        {
            _anim.SetBool("isMoving", false);
            return;
        }
        _anim.SetBool("isMoving", true);
    }
}