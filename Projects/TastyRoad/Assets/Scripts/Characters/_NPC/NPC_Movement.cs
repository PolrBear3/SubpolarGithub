using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Movement : MonoBehaviour
{
    [Header("")]
    [SerializeField] private NPC_Controller _controller;

    private Vector2 _targetPosition;
    public Vector2 targetPosition => _targetPosition;

    private Coroutine _moveCoroutine;

    private bool _roamActive;
    public bool roamActive => _roamActive;

    private bool _isLeaving;
    public bool isLeaving => _isLeaving;

    private SpriteRenderer _currentRoamArea;
    public SpriteRenderer currentRoamArea => _currentRoamArea;


    [Header("")]
    [SerializeField][Range(0, 100)] private float _defaultMoveSpeed;
    public float defaultMoveSpeed => _defaultMoveSpeed;

    private float _moveSpeed;


    [Header("")]
    [SerializeField][Range(0, 100)] private int _intervalTime;
    public int intervalTimeRange => _intervalTime;

    [SerializeField][Range(0, 100)] private int _searchAttempts;
    public int searchAttempts => _searchAttempts;


    public delegate void Event();
    public Event TargetPosition_UpdateEvent;


    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out NPC_Controller controller)) { _controller = controller; }

        _moveSpeed += _defaultMoveSpeed;
        _targetPosition = transform.position;
    }

    private void Update()
    {
        _controller.basicAnim.Idle_Move(Is_Moving());
        Movement_Update();
    }


    // Get
    public float Move_Direction()
    {
        // return left
        if (transform.position.x > _targetPosition.x) return -1f;
        // return right
        else return 1f;
    }

    public float Random_IntervalTime()
    {
        return Random.Range(0, _intervalTime + 1);
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

    public bool At_CurrentRoamArea()
    {
        return currentRoamArea.bounds.Contains(transform.position);
    }


    // Movement
    private void Movement_Update()
    {
        if (At_TargetPosition() == false)
        {
            transform.position = Vector2.MoveTowards(transform.position, _targetPosition, _moveSpeed * 0.1f * Time.deltaTime);
        }
    }

    public void Set_MoveSpeed(float setValue)
    {
        _moveSpeed = Mathf.Clamp(setValue, _defaultMoveSpeed, 100f);
    }


    // Control
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
        _controller.basicAnim.Flip_Sprite(Move_Direction());

        TargetPosition_UpdateEvent?.Invoke();
    }

    /// <summary>
    /// Attempts to find path cleared position
    /// </summary>
    public void Assign_TargetPosition(SpriteRenderer searchArea)
    {
        Main_Controller main = Main_Controller.instance;

        int attemptCount = _searchAttempts;
        Vector2 targetPosition;

        do
        {
            targetPosition = main.Random_AreaPoint(searchArea);
            bool stationPlaced = main.Is_StationArea(targetPosition);

            if (stationPlaced == false)
            {
                Assign_TargetPosition(targetPosition);
                return;
            }
            attemptCount--;
        }
        while (attemptCount > 0);

        Leave(Random_IntervalTime());
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
    public void Free_Roam(SpriteRenderer roamArea)
    {
        Free_Roam(roamArea, Random_IntervalTime());
    }
    /// <summary>
    /// Default roam area of current location
    /// </summary>
    public void Free_Roam(float startDelayTime)
    {
        SpriteRenderer roamArea = Main_Controller.instance.currentLocation.data.roamArea;
        Free_Roam(roamArea, startDelayTime);
    }

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

        if (_roamActive == false)
        {
            _moveCoroutine = null;
            yield break;
        }

        Assign_TargetPosition(roamArea);

        // repeat until free roam deactivates
        while (_roamActive == true)
        {
            // wait until NPC reaches target position
            while (At_TargetPosition() == false)
            {
                yield return null;
            }

            yield return new WaitForSeconds(Random_IntervalTime());
            if (_roamActive == false) break;

            Assign_TargetPosition(roamArea);
        }

        _moveCoroutine = null;
        yield break;
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
        _moveCoroutine = StartCoroutine(Leave_Coroutine(Random.Range(0, 2), startDelayTime));
    }

    /// <summary>
    /// 0 for left, 1 for right
    /// </summary>
    public void Leave(int direction, float startDelayTime)
    {
        if (_moveCoroutine != null) StopCoroutine(_moveCoroutine);

        _isLeaving = true;
        _moveCoroutine = StartCoroutine(Leave_Coroutine(direction, startDelayTime));
    }
    private IEnumerator Leave_Coroutine(int direction, float startDelayTime)
    {
        yield return new WaitForSeconds(startDelayTime);

        Main_Controller main = Main_Controller.instance;

        // random left or right side of camera
        Vector2 targetPosition = main.currentLocation.OuterLocation_Position(direction);

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