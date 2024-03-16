using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Movement : MonoBehaviour
{
    private NPC_Controller _controller;

    private Vector2 _targetPoint;

    [SerializeField] private float _moveSpeed;

    // UnityEngine
    private void Awake()
    {
        _controller = gameObject.GetComponent<NPC_Controller>();
    }

    private void Update()
    {
        if (Is_TargetPoint())
        {
            _controller.animationControl.Play_AnimationState("NPC_idle");
            return;
        }

        _controller.animationControl.Play_AnimationState("NPC_walk");
        Move();
    }

    // Checks
    public bool Is_TargetPoint()
    {
        float distance = Vector2.Distance(transform.position, _targetPoint);

        if (distance < 0.1f) return true;
        else return false;
    }

    // Update Movement
    public void Move()
    {
        // transform.Translate(_moveSpeed * Time.deltaTime * new Vector2(1f, 0f));
        transform.position = Vector2.MoveTowards(transform.position, _targetPoint, _moveSpeed * Time.deltaTime);
    }

    //
    public void Set_TargetPoint(Vector2 point)
    {
        _targetPoint = point;
    }
}
