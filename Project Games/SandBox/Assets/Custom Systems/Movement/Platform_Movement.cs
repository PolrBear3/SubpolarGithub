using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Platform_Movement : MonoBehaviour
{
    private Rigidbody2D _rb;

    private float _xDirection;
    [SerializeField] private float _moveSpeed;

    //
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Rigidbody2D rb)) { _rb = rb; }
    }
    private void Update()
    {
        Move();
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        _xDirection = context.ReadValue<Vector2>().x;
    }

    // Movement
    private void Move()
    {
        Debug.Log(_xDirection);
    }
}
