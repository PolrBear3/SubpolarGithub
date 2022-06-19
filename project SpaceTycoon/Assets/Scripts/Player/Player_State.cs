using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum playerStateType { health, tiredness }

[System.Serializable]
public struct PlayerState
{
    public playerStateType playerStateType;
    public float maxStateSize;
    public float currentStateSize;
}

public class Player_State : MonoBehaviour
{
    public Player_MainController playerController;

    public PlayerState[] playerStates;

    private void Start()
    {
        Set_Current_State_Level();
    }

    private void Update()
    {
        Player_Tiredness_State();
    }

    void Set_Current_State_Level()
    {
        for (int i = 0; i < playerStates.Length; i++)
        {
            playerStates[i].currentStateSize = playerStates[i].maxStateSize;
        }
    }

    // player state constructors
    bool Check_Max_Status_Size(float currentSize, float maxSize)
    {
        if (currentSize <= maxSize) { return true; }
        else return false;
    }
    bool Check_Min_Status_Size(float currentSize)
    {
        if (currentSize >= 0) { return true; }
        else return false;
    }

    void Player_Increase_Status(float healSize)
    {
        playerStates[1].currentStateSize += healSize * Time.deltaTime;
    }
    void Player_Decrease_Status(float decreseSize)
    {
        playerStates[1].currentStateSize -= decreseSize * Time.deltaTime;
    }
    void Player_Subtract_Status(float subtractSize)
    {
        playerStates[1].currentStateSize -= subtractSize;
    }

    // player state public constructors

    // player state update check
    void Player_Tiredness_State()
    {
        if (playerController.playerMovement.Player_is_Moving())
        {
            if (Check_Min_Status_Size(playerStates[1].currentStateSize))
            {
                Player_Decrease_Status(playerController.playerOutfit.currentOutfit.tirednessDepleteSize);
            }
        }
        if (playerController.playerMovement.Player_is_Jumping())
        {
            if (Check_Min_Status_Size(playerStates[1].currentStateSize))
            {
                Player_Subtract_Status(playerController.playerOutfit.currentOutfit.jumpTirednessSubtractSize);
            }
        }
        if (playerController.playerMovement.Player_is_Standing())
        {
            if (Check_Max_Status_Size(playerStates[1].currentStateSize, playerStates[1].maxStateSize))
            {
                Player_Increase_Status(playerController.playerOutfit.currentOutfit.tirednessHealSize);
            }
        }
    }
}
