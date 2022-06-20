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

public enum PlayerActionType { isDefault, isSitting, isSleeping }
[System.Serializable]
public struct PlayerAction
{
    public PlayerActionType playerActionType;
    public float actionMultiplySize;
}

public class Player_State : MonoBehaviour
{
    public Player_MainController playerController;
    public PlayerState[] playerStates;
    public PlayerAction playerCurrentAction;
    public PlayerAction[] playerActions;

    private void Start()
    {
        Set_Current_State_Level();
    }

    private void Update()
    {
        Player_isStanding();
        Player_isMoving();
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

    // action constructors
    public void Player_Action(PlayerActionType actionType)
    {
        for (int i = 0; i < playerActions.Length; i++)
        {
            if(actionType == playerActions[i].playerActionType)
            {
                playerCurrentAction = playerActions[i];
                break;
            }
        }
    }
    public bool Player_Current_Action_Check(PlayerActionType actionType)
    {
        if (actionType == playerCurrentAction.playerActionType) { return true; }
        else return false;
    }

    // player state update
    void Player_isMoving()
    {
        if (playerController.playerMovement.Movement_Check())
        {
            Player_Action(PlayerActionType.isDefault);
            Deplete_State_Size(1, playerController.playerOutfit.currentOutfit.tirednessDepleteSize);
        }
    }
    void Player_isStanding()
    {
        if (playerController.playerMovement.Standing_Check() && Player_Current_Action_Check(PlayerActionType.isDefault))
        {
            Increase_State_Size(1, playerController.playerOutfit.currentOutfit.tirednessHealSize);
        }
    }

        // single player state update (increase and deplete)
        // check if current action is increase state or deplete state
}
