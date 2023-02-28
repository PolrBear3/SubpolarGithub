using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAction
{
    public void Action();
}

[System.Serializable]
public struct Location_Data
{
    public int rowNum;
    public int columnNum;
}

public class Game_DataBase : MonoBehaviour
{
    public GameObject[] stageBoards;
    public GameObject[] playerPiece;
}
