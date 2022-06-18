using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_MousePosition : MonoBehaviour
{
    public Player_MainController playerController;
    public Transform player;
    public bool facingLeft;

    void Update()
    {
        Mouse_Position();
    }

    void Mouse_Position()
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(player.position);

        if (Input.mousePosition.x < screenPosition.x && !facingLeft)
        {
            Flip();
        }
        if (Input.mousePosition.x > screenPosition.x && facingLeft)
        {
            Flip();
        }
    }
    void Flip()
    {
        facingLeft = !facingLeft;

        transform.Rotate(0f, 180f, 0f);
    }
}
