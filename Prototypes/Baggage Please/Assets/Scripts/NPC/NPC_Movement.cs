using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Movement : MonoBehaviour
{
    private Game_Controller _gameController;
    private NPC_Controller _controller;

    private Vector2 _targetPoint;
    private float _threshold;

    [SerializeField] private float _moveSpeed;

    // UnityEngine
    private void Awake()
    {
        _gameController = FindObjectOfType<Game_Controller>();
        _controller = gameObject.GetComponent<NPC_Controller>();
    }

    private void Update()
    {
        if (Is_TargetPoint()) return;
        Move();
    }

    // Checks
    public bool Is_TargetPoint()
    {
        float distance = Vector2.Distance(transform.position, _targetPoint);
        if (distance < _threshold) return true;
        else return false;
    }

    // Update Movement
    public void Move()
    {
        transform.position = Vector2.MoveTowards(transform.position, _targetPoint, _moveSpeed * Time.deltaTime);
    }

    //
    public void Set_TargetPoint(Vector2 point)
    {
        _targetPoint = point;
    }
}
