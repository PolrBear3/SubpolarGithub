using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackCard : MonoBehaviour
{
    private Transform _followPoint;
    public Transform followPoint => _followPoint;

    private float _moveSpeed;


    // MonoBehaviour
    /*
    private void Update()
    {
        Follow_Update();
    }
    */


    // Main
    public void Set_FollowData(Transform followPoint, float moveSpeed)
    {
        _followPoint = followPoint;
        _moveSpeed = moveSpeed;
    }

    private void Follow_Update()
    {
        if (_followPoint == null) return;

        transform.position = Vector2.Lerp(transform.position, _followPoint.position, Time.deltaTime * _moveSpeed);
    }
}
