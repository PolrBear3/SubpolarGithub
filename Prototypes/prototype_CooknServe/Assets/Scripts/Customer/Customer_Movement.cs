using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer_Movement : MonoBehaviour
{
    private SpriteRenderer _sr;
    [HideInInspector] public Rigidbody2D rb;
    private Customer_Controller _customerController;

    [Header("Data")]
    public float moveSpeed;
    private Vector2 _nextPosition;

    [Header("Free Roam")]
    public float roamStartTime;
    public float roamIntervalTime;
    [HideInInspector] public bool roamActive;

    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out SpriteRenderer sr)) { _sr = sr; }
        if (gameObject.TryGetComponent(out Rigidbody2D rb)) { this.rb = rb; }
        if (gameObject.TryGetComponent(out Customer_Controller customerController)) { _customerController = customerController; }
    }
    private void Start()
    {
        Start_FreeRoam();
    }
    private void Update()
    {
        Update_NextPosition_Movement();
    }

    // Check
    public bool Is_NextPosition()
    {
        if ((Vector2)transform.position == _nextPosition) return true;
        return false;
    }

    // Custom
    private void Update_NextPosition_Movement()
    {
        if (Is_NextPosition()) return;
        transform.position = Vector2.MoveTowards(transform.position, _nextPosition, .5f * Time.deltaTime);
    }

    public void Update_NextPosition(Vector2 position)
    {
        Stop_FreeRoam();
        _nextPosition = position;
        Flip_Update();
    }

    // Flip
    public void Flip_toPlayer()
    {
        Player_Controller player = _customerController.playerController;

        if (transform.position.x > player.transform.position.x) _sr.flipX = true;
        else _sr.flipX = false;
    }
    public void Flip_Update()
    {
        if (transform.position.x > _nextPosition.x) _sr.flipX = true;
        else _sr.flipX = false;
    }

    // Move Type
    private IEnumerator Free_Roam()
    {
        yield return new WaitForSeconds(roamStartTime);

        Vector2 startPos = _customerController.gameController.dataController.Get_BoxArea_Position(0);
        Update_NextPosition(startPos);

        while (roamActive)
        {
            yield return new WaitForSeconds(roamIntervalTime);

            Vector2 nextPos = _customerController.gameController.dataController.Get_BoxArea_Position(0);
            Update_NextPosition(nextPos);
        }
    }
    public void Start_FreeRoam()
    {
        _nextPosition = transform.position;
        roamActive = true;
        StartCoroutine(Free_Roam());
    }
    public void Stop_FreeRoam()
    {
        StopAllCoroutines();
        roamActive = false;
        _nextPosition = transform.position;
    }
}