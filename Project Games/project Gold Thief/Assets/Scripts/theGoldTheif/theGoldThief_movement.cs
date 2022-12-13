using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public struct theGoldThief_movements_data
{
    public float moveSpeed;
    public float jumpPower;

    public Vector2 moveDirection;

    [HideInInspector]
    public bool isGround;
    [HideInInspector]
    public bool isMoving;
}

public class theGoldThief_movement : MonoBehaviour
{
    public theGoldThief_mainController controller;
    public theGoldThief_movements_data data;

    private void FixedUpdate()
    {
        run();
    }
    private void Update()
    {
        movement_animation();
    }

    private void movement_animation()
    {
        controller.connection.mainAnim.SetFloat("movement", Mathf.Abs(data.moveDirection.x));
    }

    public void OnRun(InputValue value)
    {
        data.moveDirection = value.Get<Vector2>();
    }
    private void run()
    {
        controller.connection.mainRB.velocity = new Vector2(data.moveDirection.x * data.moveSpeed, controller.connection.mainRB.velocity.y);
    }

    public void OnJump()
    {
        jump();
    }
    private void jump()
    {
        controller.connection.mainRB.AddForce(Vector2.up * data.jumpPower);
    }
}
