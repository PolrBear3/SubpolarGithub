using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_MousePosition : MonoBehaviour
{
    public Player_MainController playerController;
    public Transform player;
    [HideInInspector]
    public bool facingLeft;
    bool mouseFlip_Freeze = false;

    void Update()
    {
        Mouse_Position();
    }

    void Mouse_Position()
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(player.position);

        if (!mouseFlip_Freeze)
        {
            if (Input.mousePosition.x < screenPosition.x && !facingLeft)
            {
                Flip_Player();
            }
            if (Input.mousePosition.x > screenPosition.x && facingLeft)
            {
                Flip_Player();
            }
        }
    }

    public void Flip_Player()
    {
        transform.Rotate(0f, 180f, 0f);
        facingLeft = !facingLeft;
    }

    public void Freeze_MouseFlip()
    {
        mouseFlip_Freeze = true;
    }
    public void UnFreeze_MouseFlip()
    {
        mouseFlip_Freeze = false;
    }
}
