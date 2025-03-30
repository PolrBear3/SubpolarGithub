using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Movement : MonoBehaviour
{
    private Rigidbody2D _rigidBody;
    private Player_Controller _playerController;


    [Header("")]
    [SerializeField][Range(0, 100)] private float _defaultMoveSpeed;
    public float defaultMoveSpeed => _defaultMoveSpeed;

    private float _moveSpeed;
    public float moveSpeed => _moveSpeed;


    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Player_Controller playerController)) { _playerController = playerController; }
        if (gameObject.TryGetComponent(out Rigidbody2D rigidbody)) { _rigidBody = rigidbody; }

        _moveSpeed += _defaultMoveSpeed;
    }

    private void Start()
    {
        Input_Controller.instance.OnMovement += FaceDirection_Update;
    }

    private void OnDestroy()
    {
        Input_Controller.instance.OnMovement -= FaceDirection_Update;
    }

    private void Update()
    {
        _playerController.animationController.Idle_Move(Is_Moving());
    }

    private void FixedUpdate()
    {
        Movement_Update();
    }


    // Movement
    public bool Is_Moving()
    {
        if (_rigidBody.velocity != Vector2.zero) return true;
        else return false;
    }


    private void Movement_Update()
    {
        Vector2 inputDirection = Input_Controller.instance.inputDirection;

        _rigidBody.velocity = new Vector2(inputDirection.x * _moveSpeed, inputDirection.y * _moveSpeed);
    }

    private void FaceDirection_Update(Vector2 direction)
    {
        if (enabled == false) return;

        _playerController.animationController.Flip_Sprite(direction);
    }


    public void Set_MoveSpeed(float setValue)
    {
        _moveSpeed = Mathf.Clamp(setValue, _defaultMoveSpeed, 100f);
    }
}
