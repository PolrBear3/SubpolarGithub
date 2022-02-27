using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_MousePosition : MonoBehaviour
{
    public Player_MainController playerController;

    public Transform player;

    void Update()
    {
        Mouse_Position();
    }

    bool facingRight = false;
    void Mouse_Position()
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(player.position);

        if (Input.mousePosition.x < screenPosition.x && !facingRight)
        {
            Flip();
        }
        if (Input.mousePosition.x > screenPosition.x && facingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        facingRight = !facingRight;

        transform.Rotate(0f, 180f, 0f);
    }
}
