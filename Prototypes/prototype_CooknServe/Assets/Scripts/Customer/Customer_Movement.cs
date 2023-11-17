using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer_Movement : MonoBehaviour
{
    private SpriteRenderer _sr;

    private Customer_Controller _customerController;

    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out SpriteRenderer sr)) { _sr = sr; }
        if (gameObject.TryGetComponent(out Customer_Controller customerController)) { _customerController = customerController; }
    }

    // Custom
    public void Flip_toPlayer()
    {
        Player_Controller player = _customerController.playerController;

        if (transform.position.x > player.transform.position.x) _sr.flipX = true;
        else _sr.flipX = false;
    }
}