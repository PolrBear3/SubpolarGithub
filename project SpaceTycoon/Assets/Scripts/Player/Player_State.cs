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

public enum PlayerActionType { isSitting, isSleeping }
[System.Serializable]
public struct PlayerAction
{
    public PlayerActionType playerActionType;
    public bool actionActive;
    public float actionMultiplySize;
}

public class Player_State : MonoBehaviour
{
    public Player_MainController playerController;
    public PlayerState[] playerStates;
    public PlayerAction[] playerActions;

    private void Start()
    {
        Set_Current_State_Level();
    }

    private void Update()
    {
        Player_isStanding();
        Player_isMoving();
        Test_Sit();
    }

    void Set_Current_State_Level()
    {
        for (int i = 0; i < playerStates.Length; i++)
        {
            playerStates[i].currentStateSize = playerStates[i].maxStateSize;
        }
    }

    // state constructors
    bool Set_Max_Size(int playerStateNum)
    {
        if (playerStates[playerStateNum].currentStateSize <= playerStates[playerStateNum].maxStateSize) { return true; }
        else return false;
    }
    bool Set_Min_Size(int playerStateNum)
    {
        if (playerStates[playerStateNum].currentStateSize >= 0) { return true; }
        else return false;
    }

    public void Increase_State_Size(int playerStateNum, float increaseSize)
    {
        if (Set_Max_Size(playerStateNum))
        {
            playerStates[playerStateNum].currentStateSize += Time.deltaTime * increaseSize;
        }
    }
    public void Deplete_State_Size(int playerStateNum, float depleteSize)
    {
        if (Set_Min_Size(playerStateNum))
        {
            playerStates[playerStateNum].currentStateSize -= Time.deltaTime * depleteSize;
        }
    }

    public void Add_State_Size(int playerStateNum, float increaseSize)
    {
        if (Set_Max_Size(playerStateNum))
        {
            playerStates[playerStateNum].currentStateSize += increaseSize;
        }
    }
    public void Subtract_State_Size(int playerStateNum, float subtractSize)
    {
        if (Set_Min_Size(playerStateNum))
        {
            playerStates[playerStateNum].currentStateSize -= subtractSize;
        }
    }

    public void Player_Action(PlayerActionType actionType)
    {
        for (int i = 0; i < playerActions.Length; i++)
        {
            if(actionType == playerActions[i].playerActionType)
            {
                playerActions[i].actionActive = true;
                break;
            }
        }
    }

    // player state update
    void Player_isMoving()
    {
        if (playerController.playerMovement.Movement_Check())
        {
            Deplete_State_Size(1, playerController.playerOutfit.currentOutfit.tirednessDepleteSize);
        }
    }
    void Player_isStanding()
    {
        if (playerController.playerMovement.Standing_Check() && !Player_OnAction())
        {
            Increase_State_Size(1, playerController.playerOutfit.currentOutfit.tirednessHealSize);
        }
    }
    
    bool Player_OnAction()
    {
        for (int i = 0; i < playerActions.Length; i++)
        {
            if (playerActions[i].actionActive == true)
            {
                return true;
            }
        }
        return false;
    }



    // test
    void Test_Sit()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Player_Action(PlayerActionType.isSitting);
        }
    }
}
