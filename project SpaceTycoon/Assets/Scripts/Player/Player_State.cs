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

    private void Update()
    {

    }

    void Start_Set_Current_State_Level()
    {
        for (int i = 0; i < playerStates.Length; i++)
        {
            playerStates[i].currentStateSize = playerStates[i].maxStateSize;
        }
    }
}
