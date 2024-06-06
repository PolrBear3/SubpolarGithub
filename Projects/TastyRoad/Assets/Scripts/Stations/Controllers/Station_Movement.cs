using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Station_Movement : MonoBehaviour
{
    private Station_Controller _stationController;

    private Rigidbody2D _rigidBody;
    private Vector2 _currentDirection;

    [SerializeField] private GameObject _movementArrows;
    public GameObject movementArrows => _movementArrows;


    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Station_Controller stationController)) { _stationController = stationController; }
        if (gameObject.TryGetComponent(out Rigidbody2D rigidbody)) { _rigidBody = rigidbody; }
    }

    private void Start()
    {
        _movementArrows.SetActive(true);
    }

    private void Update()
    {
        RestrictBlink_Update();
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


    // for fixed update
    private void Rigidbody_Move()
    {
        _rigidBody.velocity = new Vector2(_currentDirection.x * 3f, _currentDirection.y * 3f);
    }


    /// <returns>
    /// True if current snap position is not claimed and not in a location restricted area
    /// </returns>
    public bool PositionSet_Available()
    {
        Vector2 snapPosition = Main_Controller.SnapPosition(transform.position);
        if (_stationController.mainController.Position_Claimed(snapPosition)) return false;

        Location_Controller location = _stationController.mainController.currentLocation;
        if (location.Restricted_Position(snapPosition)) return false;

        return true;
    }

    /// <summary>
    /// Toggle active on claimed positions
    /// </summary>
    private void RestrictBlink_Update()
    {
        _stationController.RestrictionBlink_Toggle(!PositionSet_Available());
    }

    /// <summary>
    /// Disables movement and set current station after movement is controlled
    /// </summary>
    public void Set_Position()
    {
        if (PositionSet_Available() == false) return;

        Load_Position();
    }

    /// <summary>
    /// Disables movement and set current station when game is loaded or spawned manually
    /// </summary>
    public void Load_Position()
    {
        Vector2 snapPosition = Main_Controller.SnapPosition(transform.position);

        _stationController.Action1_Event -= Set_Position;

        _stationController.PlayerInput_Activation(false);

        _stationController.TransparentBlink_Toggle(false);
        _movementArrows.SetActive(false);

        MathRound_Snap_Position();

        _rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
        enabled = false;
    }


    //
    private void MathRound_Snap_Position()
    {
        Vector2 snapPosition = Main_Controller.SnapPosition(transform.position);

        transform.localPosition = snapPosition;
        _stationController.mainController.Claim_Position(snapPosition);
    }
}