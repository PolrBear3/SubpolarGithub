using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Manager : MonoBehaviour
{
    public Game_DataBase dataBase;
    
    public Stage_Board stageBoard;
    public Player_Piece playerPiece;

    private void Start()
    {
        Spawn_StageBoard();
        Spawn_PlayerPiece();
    }

    public void Spawn_StageBoard()
    {
        var stage = Instantiate(dataBase.stageBoards[0], Vector2.zero, Quaternion.identity);
        stageBoard = stage.GetComponent<Stage_Board>();
        stageBoard.Set_Tile_Data();
    }
    public void Spawn_PlayerPiece()
    {
        var player = Instantiate(dataBase.playerPieces[0], Vector2.zero, Quaternion.identity);
        playerPiece = player.GetComponent<Player_Piece>();
        stageBoard.Set_PlayerPiece(player);
    }
}