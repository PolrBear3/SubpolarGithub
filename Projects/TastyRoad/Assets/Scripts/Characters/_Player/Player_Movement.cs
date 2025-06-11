using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using FMOD.Studio;

public class Player_Movement : MonoBehaviour
{
    private Rigidbody2D _rigidBody;
    private Player_Controller _playerController;


    [Space(20)]
    [SerializeField][Range(0, 100)] private float _defaultMoveSpeed;
    public float defaultMoveSpeed => _defaultMoveSpeed;

    [SerializeField] [Range(0, 10)] private float _movementInvokeDuration;

    [Space(60)] 
    [SerializeField] private Ability_ScrObj _movementSpeedAbility;
    
    
    private float _moveSpeed;
    public float moveSpeed => _moveSpeed;

    public Action OnMovement;
    private Coroutine _movementCoroutine;
    

    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Player_Controller playerController)) { _playerController = playerController; }
        if (gameObject.TryGetComponent(out Rigidbody2D rigidbody)) { _rigidBody = rigidbody; }

        _moveSpeed += _defaultMoveSpeed;
    }

    private void Start()
    {
        // subscriptions
        OnMovement += () => AbilityManager.IncreasePoint(_movementSpeedAbility, 1);
        
        Input_Controller.instance.OnActionMapUpdate += Force_MovementRestriction;
    }

    private void OnDestroy()
    {
        // subscriptions
        Input_Controller.instance.OnActionMapUpdate -= Force_MovementRestriction;
    }

    private void Update()
    {
        _playerController.animationController.Idle_Move(Is_Moving());
        FaceDirection_Update(_rigidBody.velocity);
        Invoke_OnMovement();
    }

    private void FixedUpdate()
    {
        Movement_Update();
    }


    // Movement
    public bool Is_Moving()
    {
        if (_rigidBody.velocity != Vector2.zero) return true;
        return false;
    }


    private void Movement_Update()
    {
        Vector2 inputDirection = Input_Controller.instance.movementDirection;

        _rigidBody.velocity = new Vector2(inputDirection.x * _moveSpeed, inputDirection.y * _moveSpeed);
    }
    
    private void FaceDirection_Update(Vector2 direction)
    {
        if (enabled == false) return;

        _playerController.animationController.Flip_Sprite(direction);
    }

    private void Invoke_OnMovement()
    {
        if (Is_Moving() == false)
        {
            if (_movementCoroutine == null) return;
            
            StopCoroutine(_movementCoroutine);
            _movementCoroutine = null;

            return;
        }
        
        if (_movementCoroutine != null) return;
        _movementCoroutine = StartCoroutine(Invoke_OnMovement_Coroutine());
    }
    private IEnumerator Invoke_OnMovement_Coroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_movementInvokeDuration);
            OnMovement?.Invoke();
        }
    }
    
    
    public void Force_MovementRestriction()
    {
        _rigidBody.velocity = Vector2.zero;
        _playerController.animationController.Idle_Move(false);
        
        if (_movementCoroutine == null) return;
        
        StopCoroutine(_movementCoroutine);
        _movementCoroutine = null;
    }

    public void Set_MoveSpeed(float setValue)
    {
        _moveSpeed = Mathf.Clamp(setValue, _defaultMoveSpeed, 100f);
    }
}
