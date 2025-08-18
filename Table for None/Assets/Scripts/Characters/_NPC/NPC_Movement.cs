using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Movement : MonoBehaviour
{
    [Space(20)]
    [SerializeField] private NPC_Controller _controller;

    [Space(20)]
    [SerializeField][Range(0, 100)] private float _defaultMoveSpeed;
    public float defaultMoveSpeed => _defaultMoveSpeed;

    [SerializeField][Range(0, 100)] private float _intervalTime;
    public float intervalTime => _intervalTime;

    [Space(20)] 
    [SerializeField] [Range(0, 100)] private float _updateDelayTime;

    
    private float _moveSpeed;

    private Vector2 _targetPosition;
    public Vector2 targetPosition => _targetPosition;

    private Coroutine _moveCoroutine;

    private bool _roamActive;
    public bool roamActive => _roamActive;

    private bool _isLeaving;
    public bool isLeaving => _isLeaving;

    private SpriteRenderer _currentRoamArea;
    public SpriteRenderer currentRoamArea => _currentRoamArea;
    
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


    // Position
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
    /// Assigns to random point inside searchArea
    /// </summary>
    private void Assign_TargetPosition(SpriteRenderer searchArea)
    {
        Assign_TargetPosition(Utility.Random_BoundPoint(searchArea.bounds));
    }


    // Free Roam  
    public void Stop_FreeRoam()
    {
        if (_moveCoroutine != null) StopCoroutine(_moveCoroutine);

        _roamActive = false;
        _targetPosition = transform.position;
    }

    public void Update_RoamArea(SpriteRenderer assignArea)
    {
        _currentRoamArea = assignArea;
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

        while (_roamActive == true)
        {
            Assign_TargetPosition(roamArea);

            while (At_TargetPosition() == false) yield return null;

            if (_roamActive == false) break;
            yield return new WaitForSeconds(Random_IntervalTime());
        }

        _moveCoroutine = null;
        yield break;
    }

    /// <summary>
    /// Redirects to opposite direction if restricted area is detected from Location_Controller
    /// </summary>
    public void CurrentLocation_FreeRoam(float startDelayTime)
    {
        if (_moveCoroutine != null) StopCoroutine(_moveCoroutine);

        _roamActive = true;
        _currentRoamArea = Main_Controller.instance.currentLocation.data.roamArea;
        _moveCoroutine = StartCoroutine(CurrentLocation_FreeRoam_Coroutine(startDelayTime));
    }
    private IEnumerator CurrentLocation_FreeRoam_Coroutine(float startDelayTime)
    {
        yield return new WaitForSeconds(startDelayTime);

        if (_roamActive == false)
        {
            _moveCoroutine = null;
            yield break;
        }

        Location_Controller currentLocation = Main_Controller.instance.currentLocation;
        SpriteRenderer locationRoamArea = currentLocation.data.roamArea;

        while (_roamActive == true)
        {
            Assign_TargetPosition(locationRoamArea);
            
            while (At_TargetPosition() == false)
            {
                float updateDelayTime = Mathf.Clamp(_updateDelayTime, 0.5f, _updateDelayTime);
                yield return new WaitForSeconds(updateDelayTime);
                
                SpriteRenderer restrictedArea = currentLocation.Restricted_Area(transform.position);
                if (restrictedArea == null) continue;

                Bounds roamBounds = locationRoamArea.bounds;
                float directionValue = restrictedArea.bounds.center.x;
                
                float randXPoint;
                float randYPoint = Random.Range(roamBounds.min.y, roamBounds.max.y);
                
                if (transform.position.x >= directionValue)
                {
                    // pick between current position x and right edge of current location
                    randXPoint = Random.Range(transform.position.x, roamBounds.max.x);
                }
                else
                {
                    // pick between left edge of current location and current position x
                    randXPoint = Random.Range(roamBounds.min.x, transform.position.x);
                }

                Assign_TargetPosition(transform.position);
                yield return new WaitForSeconds(Random_IntervalTime());
                
                Assign_TargetPosition(new Vector2(randXPoint, randYPoint));
                continue;
            }

            if (_roamActive == false) break;
            yield return new WaitForSeconds(Random_IntervalTime());
        }

        _moveCoroutine = null;
        yield break;
    }
    
    /// <summary>
    /// Excludes outer spawn range from current Location_Controller
    /// </summary>
    public void CurrentLocation_FreeRoam(SpriteRenderer roamArea, float startDelayTime)
    {
        if (_moveCoroutine != null) StopCoroutine(_moveCoroutine);
        
        _roamActive = true;
        _currentRoamArea = roamArea;
        _moveCoroutine = StartCoroutine(CurrentLocation_FreeRoam_Coroutine(roamArea, startDelayTime));
    }
    private IEnumerator CurrentLocation_FreeRoam_Coroutine(SpriteRenderer roamArea, float startDelayTime)
    {
        Location_Controller currentLocation = Main_Controller.instance.currentLocation;

        yield return new WaitForSeconds(startDelayTime);

        if (_roamActive == false)
        {
            _moveCoroutine = null;
            yield break;
        }
        
        while (_roamActive == true)
        {
            Assign_TargetPosition(roamArea);

            while (currentLocation.Is_OuterSpawnPoint(_targetPosition))
            {
                Assign_TargetPosition(roamArea);
            }

            while (At_TargetPosition() == false) yield return null;

            if (_roamActive == false) break;
            yield return new WaitForSeconds(Random_IntervalTime());
        }

        _moveCoroutine = null;
        yield break;
    }


    // Remove
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