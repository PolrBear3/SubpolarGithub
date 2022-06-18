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

    public List<PlayerState> playerStates = new List<PlayerState>();

    private void Start()
    {

    }

    private void Update()
    {

    }

    public void Player_is_Moving()
    {

    }

    public void Player_is_Sitting()
    {

    }

    public void Player_is_Sleeping()
    {

    }
}
