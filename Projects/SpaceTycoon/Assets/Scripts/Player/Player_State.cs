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
        Start_Set_Current_State_Level();
    }

    void Start_Set_Current_State_Level()
    {
        playerStates[0].currentStateSize = playerStates[0].maxStateSize;
        playerStates[1].currentStateSize = 0;
    }

    // state minimum maximum check
    public bool State_CurrentlyNot_Max(int playerStateArrayNumber)
    {
        var playerState = playerStates[playerStateArrayNumber];

        if (playerState.currentStateSize >= playerState.maxStateSize) { return false; }
        else return true;
    }
    public bool State_CurrentlyNot_Min(int playerStateArrayNumber)
    {
        var playerState = playerStates[playerStateArrayNumber];

        if (playerState.currentStateSize <= 0) { return false; }
        else return true;
    }
    
    // state update constructors
    public void Increase_State_Size(int playerStateArrayNumber, float sizeAmount)
    {
        if (State_CurrentlyNot_Max(playerStateArrayNumber))
        {
            playerStates[playerStateArrayNumber].currentStateSize += Time.deltaTime * sizeAmount;
        }
    }
    public void Decrease_State_Size(int playerStateArrayNumber, float sizeAmount)
    {
        if (State_CurrentlyNot_Min(playerStateArrayNumber))
        {
            playerStates[playerStateArrayNumber].currentStateSize -= Time.deltaTime * sizeAmount;
        }
    }

    // state trigger constructors
    public void Add_State_Size(int playerStateArrayNumber, float sizeAmount)
    {
        if (State_CurrentlyNot_Max(playerStateArrayNumber))
        {
            playerStates[playerStateArrayNumber].currentStateSize += sizeAmount;
        }
    }
    public void Subtract_State_Size(int playerStateArrayNumber, float sizeAmount)
    {
        if (State_CurrentlyNot_Min(playerStateArrayNumber))
        {
            playerStates[playerStateArrayNumber].currentStateSize -= sizeAmount;
        }
    }
}
