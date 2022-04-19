using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch_System : MonoBehaviour
{
    public int playerNum;

    public void Player_Check()
    {
        switch (playerNum)
        {
            case 0:
                Debug.Log("No players online");
                break;
            case 1:
                Debug.Log("Hello, player 1");
                break;
            case 2:
                Debug.Log("2 players Online");
                break;
            default:
                Debug.Log("More than 2 players online");
                break;
        }
    }
}
