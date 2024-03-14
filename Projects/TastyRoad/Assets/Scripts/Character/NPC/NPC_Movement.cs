using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Movement : MonoBehaviour
{
    private NPC_Controller _controller;

    private Vector2 _targetPosition;

    private Coroutine _moveCoroutine;

    private bool _roamActive;
    public bool roamActive => _roamActive;

    private bool _isLeaving;
    public bool isLeaving => _isLeaving;

    private SpriteRenderer _currentRoamArea;
    public SpriteRenderer currentRoamArea => _currentRoamArea;

    [SerializeField] private float _moveSpeed;
    [SerializeField] private Vector2 _intervalTimeRange;



    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out NPC_Controller controller)) { _controller = controller; }
    }

    private void Start()
    {
        _targetPosition = transform.position;

        // start roam at current location
        Free_Roam(_controller.mainController.currentLocation.roamArea, 0f);
    }

    private void Update()
    {
        _controller.animationController.Idle_Move(Is_Moving());
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
    public bool At_TargetPosition(Vector2 targetPosition)
    {
        float threshold = 0.1f;
        float distance = Vector2.Distance(transform.position, targetPosition);
        return distance < threshold;
    }

    // Movement Update
    private void TargetPosition_Movement()
    {
        if (At_TargetPosition() == false)
        {
            transform.position = Vector2.MoveTowards(transform.position, _targetPosition, _moveSpeed * 0.1f * Time.deltaTime);
        }
    }



    /// <summary>
    /// updates the current roam area variable
    /// </summary>
    public void Update_RoamArea(SpriteRenderer assignArea)
    {
        _currentRoamArea = assignArea;
    }

    /// <summary>
    /// Moves to assign position
    /// </summary>
    public void Assign_TargetPosition(Vector2 assignPosition)
    {
        _targetPosition = assignPosition;

        _controller.animationController.Flip_Sprite(Move_Direction());
    }

    /// <summary>
    /// Moves to assign position, after roamReturnTime pass by, returns back to Free roam
    /// </summary>
    public void Assign_TargetPosition(Vector2 assignPosition, float roamReturnTime, SpriteRenderer roamReturnArea)
    {
        _moveCoroutine = StartCoroutine(Assign_TargetPosition_Coroutine(assignPosition, roamReturnTime, roamReturnArea));
    }
    private IEnumerator Assign_TargetPosition_Coroutine(Vector2 assignPosition, float roamReturnTime, SpriteRenderer roamReturnArea)
    {
        Assign_TargetPosition(assignPosition);

        while (At_TargetPosition() == false)
        {
            yield return null;
        }

        Free_Roam(roamReturnArea, roamReturnTime);
    }

    /// <summary>
    /// Updates and moves to a random target position inside roam area on every interval time
    /// </summary>
    public void Free_Roam(SpriteRenderer roamArea, float startDelayTime)
    {
        if (_moveCoroutine != null) StopCoroutine(_moveCoroutine);

        _roamActive = true;
        _currentRoamArea = roamArea;
        _moveCoroutine = StartCoroutine(Free_Roam_Coroutine(roamArea, startDelayTime));
    }
    private IEnumerator Free_Roam_Coroutine(SpriteRenderer roamArea, float startDelayTime)
    {
        yield return new WaitForSeconds(startDelayTime);

        // start new random position
        Assign_TargetPosition(Main_Controller.Random_AreaPoint(roamArea));

        // repeat until free roam deactivates
        while (_roamActive == true)
        {
            // wait until NPC reaches target position
            while (At_TargetPosition() == false)
            {
                yield return null;
            }

            float randIntervalTime = Random.Range(_intervalTimeRange.x, _intervalTimeRange.y);
            yield return new WaitForSeconds(randIntervalTime);

            // update new random position
            Assign_TargetPosition(Main_Controller.Random_AreaPoint(roamArea));
        }
    }

    public void Stop_FreeRoam()
    {
        if (_moveCoroutine != null) StopCoroutine(_moveCoroutine);

        _roamActive = false;
        _targetPosition = transform.position;
    }



    public void Leave(float startDelayTime)
    {
        if (_moveCoroutine != null) StopCoroutine(_moveCoroutine);

        _isLeaving = true;
        _moveCoroutine = StartCoroutine(Leave_Coroutine(startDelayTime));
    }
    private IEnumerator Leave_Coroutine(float startDelayTime)
    {
        yield return new WaitForSeconds(startDelayTime);

        Main_Controller main = _controller.mainController;

        // random left or right side of camera
        Vector2 targetPosition = main.OuterCamera_Position(Random.Range(0, 2));

        // assign target position
        Assign_TargetPosition(targetPosition);

        // wait until npc reaches outer camera position
        while (At_TargetPosition(targetPosition) == false)
        {
            yield return null;
        }

        // untrack, destroy
        main.UnTrack_CurrentCharacter(gameObject);
        Destroy(gameObject);
    }
}