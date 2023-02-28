using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Manager : MonoBehaviour
{
    public Game_DataBase dataBase;
    
    public Stage_Board stageBoard;

    private void Start()
    {
        Spawn_StageBoard(0);
        Spawn_PlayerPiece(0);
    }

    public void Spawn_StageBoard(int stageDataNum)
    {
        var stage = Instantiate(dataBase.stageBoards[stageDataNum], Vector2.zero, Quaternion.identity);
        stageBoard = stage.GetComponent<Stage_Board>();
        stageBoard.Connect_Manager(this);
        stageBoard.Set_Tile_Data();
    }
    public void Spawn_PlayerPiece(int pieceDataNum)
    {
        var player = Instantiate(dataBase.playerPiece[pieceDataNum], Vector2.zero, Quaternion.identity);
        player.transform.parent = stageBoard.startingTile.groundPoint.transform;
        player.transform.localPosition = Vector2.zero;

        var boardPiece = player.GetComponent<Board_Piece>();
        boardPiece.Connect_Manager(this);

        boardPiece.Update_Location_Data(stageBoard.startingTile.data);
    }
}