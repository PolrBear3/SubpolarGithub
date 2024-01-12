using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Movement : MonoBehaviour
{
    private NPC_Controller _controller;

    private Vector2 _targetPosition;

    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _roamIntervalTime;

    private Coroutine _roamCoroutine;
    private bool _roamActive;

    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out NPC_Controller controller)) { _controller = controller; }
    }

    private void Start()
    {
        _targetPosition = transform.position;

        Free_Roam();
    }

    private void Update()
    {
        _controller.animationController.Idle_Move(Is_Moving());
    }

    private void FixedUpdate()
    {
        TargetPosition_Movement();
    }

    // Get
    public float Move_Direction()
    {
        // return left
        if (transform.position.x > _targetPosition.x) return -1f;
        // return right
        else return 1f;
    }

    // Check
    public bool Is_Moving()
    {
        if (At_TargetPosition() == false) return true;
        else return false;
    }

    public bool At_TargetPosition()
    {
        float threshold = 0.1f;
        float distance = Vector2.Distance(transform.position, _targetPosition);
        return distance < threshold;
    }

    // Movement Updates
    private void TargetPosition_Movement()
    {
        if (At_TargetPosition() == false)
        {
            transform.position = Vector2.MoveTowards(transform.position, _targetPosition, _moveSpeed * 0.1f * Time.deltaTime);
        }
    }

    // Assign Movement
    public void Assign_TargetPosition(Vector2 newPosition)
    {
        _targetPosition = newPosition;

        _controller.animationController.Flip_Sprite(Move_Direction());
    }

    // Free Roam Movement
    private IEnumerator Free_Roam_Coroutine()
    {
        // repeat until free roam deactivates
        while (_roamActive == true)
        {
            // wait until NPC reaches target position
            while (At_TargetPosition() == false)
            {
                yield return null;
            }

            yield return new WaitForSeconds(_roamIntervalTime);

            Vector2 roamTargetPos = _controller.mainController.currentLocation.Random_RoamArea(0);
            Assign_TargetPosition(roamTargetPos);
        }
    }
    public void Free_Roam()
    {
        _roamActive = true;
        _roamCoroutine = StartCoroutine(Free_Roam_Coroutine());
    }

    public void Cancel_FreeRoam()
    {
        if (_roamCoroutine != null)
        {
            _roamActive = false;
            StopCoroutine(_roamCoroutine);
        }

        _targetPosition = transform.position;
    }
}