using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public struct TheGoldThief_Movements_Data
{
    public float moveSpeed;
    public float jumpPower;

    public float footRadius;
    public float fallThreshold;

    public Vector2 moveDirection;
    public bool facingLeft;
}

public class TheGoldThief_Movement : MonoBehaviour
{
    public TheGoldThief_MainController controller;
    public TheGoldThief_Movements_Data data;

    private void FixedUpdate()
    {
        Run();
    }
    private void Update()
    {
        Movement_Animation();
        Jump_Animation();
        Flip_Check();
    }

    // character state check functions
    private bool IsOnGround()
    {
        if (Physics2D.OverlapCircle(controller.connection.footTransform.position, data.footRadius, controller.connection.groundMask))
        {
            return true;
        }
        else return false;
    }
    private bool IsMoving()
    {
        if (controller.connection.mainRB.velocity.magnitude != 0)
        {
            return true;
        }
        else return false;
    }
    private bool IsFalling()
    {
        if (controller.connection.mainRB.velocity.y < data.fallThreshold)
        {
            return true;
        }
        else return false;
    }

    // animation updates
    private void Movement_Animation()
    {
        controller.connection.mainAnim.SetFloat("movement", Mathf.Abs(data.moveDirection.x));
    }
    private void Jump_Animation()
    {
        if (!IsOnGround())
        {
            controller.connection.mainAnim.SetBool("jump", true);
        }
        else
        {
            controller.connection.mainAnim.SetBool("jump", false);
        }
    }

    // character facing side
    private void Flip()
    {
        Vector3 x = controller.connection.mainTransform.localScale;
        x.x *= -1;
        controller.connection.mainTransform.localScale = x;

        data.facingLeft = !data.facingLeft;
    }
    private void Flip_Check()
    {
        // running left
        if (controller.connection.mainRB.velocity.x < 0 && !data.facingLeft)
        {
            Flip();
        }
        if (controller.connection.mainRB.velocity.x > 0 && data.facingLeft)
        {
            Flip();
        }
    }

    // Running
    public void OnRun(InputValue value)
    {
        data.moveDirection = value.Get<Vector2>();
    }
    private void Run()
    {
        controller.connection.mainRB.velocity = new Vector2(data.moveDirection.x * data.moveSpeed, controller.connection.mainRB.velocity.y);
    }

    // Jumping
    public void OnJump()
    {
        Jump();
    }
    private void Jump()
    {
        if (IsOnGround())
        {
            controller.connection.mainRB.AddForce(Vector2.up * (data.jumpPower * 100));
        }
    }
}