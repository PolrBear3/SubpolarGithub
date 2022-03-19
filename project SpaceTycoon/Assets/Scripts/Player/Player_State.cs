using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_State : MonoBehaviour
{
    private void Update()
    {
        Player_State_Check();
    }

    public Player_MainController playerController;

    public static bool player_isFacing_Left = false;
    public static bool player_isMoving = false;

    public static bool player_isSitting = false;
    public static bool player_isSleeping = false;

    void Player_State_Check()
    {
        // if player is moving left or right or pressed the jump key W 
        if (playerController.playerMovement.horizontal > 0 || playerController.playerMovement.horizontal < 0 || Input.GetKeyDown(KeyCode.W))
        {
            player_isMoving = true;
        }
        else
        {
            player_isMoving = false;
        }

        if (player_isMoving == true)
        {
            player_isSitting = false;
            player_isSleeping = false;
        }
    }
}
