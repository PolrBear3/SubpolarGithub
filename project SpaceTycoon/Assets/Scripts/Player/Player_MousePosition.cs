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

    void Mouse_Position()
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(player.position);

        if (Input.mousePosition.x < screenPosition.x && !Player_State.player_isFacing_Left)
        {
            if (Player_State.player_isSleeping == false)
            {
                if (Player_State.player_isSitting == false)
                {
                    Flip();
                }
            }
        }
        if (Input.mousePosition.x > screenPosition.x && Player_State.player_isFacing_Left)
        {
            if (Player_State.player_isSleeping == false)
            {
                if (Player_State.player_isSitting == false)
                {
                    Flip();
                }
            }
        }
    }

    void Flip()
    {
        Player_State.player_isFacing_Left = !Player_State.player_isFacing_Left;

        transform.Rotate(0f, 180f, 0f);
    }
}
