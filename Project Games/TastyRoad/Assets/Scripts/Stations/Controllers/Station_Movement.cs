using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Station_Movement : MonoBehaviour
{
    private Station_Controller _stationController;

    private Rigidbody2D _rigidBody;
    private Vector2 _currentDirection;

    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Station_Controller stationController)) { _stationController = stationController; }
        if (gameObject.TryGetComponent(out Rigidbody2D rigidbody)) { _rigidBody = rigidbody; }
    }

    private void FixedUpdate()
    {
        Rigidbody_Move();
    }

    // InputSystem
    private void OnMovement(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        _currentDirection = input;
    }

    //
    public Vector2 Current_SnapPosition()
    {
        float snapX = (float)Mathf.Round(transform.localPosition.x);
        float snapY = (float)Mathf.Round(transform.localPosition.y);

        Vector2 snapPosition = new(snapX, snapY);

        return snapPosition;
    }

    // for fixed update
    private void Rigidbody_Move()
    {
        _rigidBody.velocity = new Vector2(_currentDirection.x * 2f, _currentDirection.y * 2f);
    }

    // set position restriction color toggle for detection controller event
    public void SetPosition_RestrictionToggle()
    {
        _stationController.Restriction_Toggle(!_stationController.detection.onInteractArea);
    }

    // Disables movement and set current station
    public void Set_Position()
    {
        if (_stationController.detection.onInteractArea == false) return;
        if (_stationController.mainController.Position_Claimed(Current_SnapPosition()) == true) return;

        _stationController.PlayerInput_Toggle(false);

        _stationController.Interact_Event -= Set_Position;
        _stationController.detection.InteractArea_Event -= SetPosition_RestrictionToggle;

        _stationController.Indicator_Toggle(false);

        MathRound_Snap_Position();

        _rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
        enabled = false;
    }

    private void MathRound_Snap_Position()
    {
        Vector2 snapPosition = Current_SnapPosition();

        _stationController.mainController.Claim_Position(snapPosition);
        transform.localPosition = snapPosition;
    }
}