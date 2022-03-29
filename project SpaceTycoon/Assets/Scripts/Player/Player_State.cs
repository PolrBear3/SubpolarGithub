using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_State : MonoBehaviour
{
    private void Start()
    {
        player_currentHealth = player_maxHealth;
    }

    private void Update()
    {
        Player_State_Check();
        Health();
        Tiredness();
    }

    public Player_MainController playerController;

    public static bool player_isFacing_Left = false;
    public static bool player_isMoving = false;

    public static bool player_isSitting = false;
    public static bool player_isSleeping = false;

    public static float player_maxHealth = 100f;
    public static float player_currentHealth;

    public static float player_maxTirednessLevel = 100f;
    public static float player_currentTirednessLevel = 0f;

    void Player_State_Check()
    {
        // if player is moving left or right or pressed the jump key W, player is moving and not sitting or sleeping
        if (playerController.playerMovement.horizontal > 0 || playerController.playerMovement.horizontal < 0 || Input.GetKeyDown(KeyCode.W))
        {
            player_isMoving = true;
            player_isSitting = false;
            player_isSleeping = false;
            player_currentTirednessLevel += 1f * Time.deltaTime; 
        }
        else
        {
            player_isMoving = false;
        }

        // if player is sleeping, cannot interact with any object
        if (player_isSleeping == true)
        {
            playerController.playerHand.SetActive(false);
        }
        else
        {
            playerController.playerHand.SetActive(true);
        }
    }

    void Health()
    {
        // health max and min
        if (player_currentHealth >= player_maxHealth)
        {
            player_currentHealth = player_maxHealth;
        }
        if (player_currentHealth <= 0f)
        {
            player_currentHealth = 0f;
        }
    }

    void Tiredness()
    {
        // tiredness max and min
        if(player_currentTirednessLevel >= player_maxTirednessLevel)
        {
            player_currentTirednessLevel = player_maxTirednessLevel;
        }
        if (player_currentTirednessLevel <= 0f)
        {
            player_currentTirednessLevel = 0f;
        }

        // if tiredness reaches 100, health goes down 0.5 each second
        if (player_currentTirednessLevel == player_maxTirednessLevel)
        {
            player_currentHealth -= 3f * Time.deltaTime;
        }

        // if player is sitting, tiredness decreases 1 each sec
        if (player_isSitting)
        {
            player_currentTirednessLevel -= 1f * Time.deltaTime;
        }

        // if player is sleeping, tiredness decreases 2 each sec
        if (player_isSleeping)
        {
            player_currentTirednessLevel -= 2f * Time.deltaTime;
        }

        // if player is wearing pajamas && player_isSleeping >> player_currentTirednessLevel -= 4f * Time.deltaTime;
    }
}
