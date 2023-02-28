using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour, IAction
{
    private Action_Controller controller;

    private void Awake()
    {
        Connect_ActionController();
    }
    public void Action()
    {
        Move_BoardPiece();
    }

    private void Connect_ActionController()
    {
        controller = gameObject.GetComponent<Action_Controller>();
    }

    private void Move_BoardPiece()
    {
        int actionID = controller.action.actionID;
        var boardPiece = controller.boardPiece;
        var boardPieceData = boardPiece.data;
        var currentStageBoard = controller.boardPiece.manager.stageBoard;
        Location_Data nextLocation = boardPieceData;

        // right
        if (actionID == 1111)
        {
            nextLocation.rowNum = boardPieceData.rowNum + 1;
        }
        // bottom
        else if (actionID == 1112)
        {
            nextLocation.columnNum = boardPieceData.columnNum + 1;
        }
        // left
        else if (actionID == 1113)
        {
            nextLocation.rowNum = boardPieceData.rowNum - 1;
        }
        // top
        else if (actionID == 1114)
        {
            nextLocation.columnNum = boardPieceData.columnNum - 1;
        }

        if (!currentStageBoard.In_StageBoard(nextLocation)) return;

        var moveLocation = currentStageBoard.Find_Tile(nextLocation).groundPoint.transform;

        // move animation

        boardPiece.transform.parent = moveLocation;
        LeanTween.moveLocal(boardPiece.gameObject, Vector2.zero, controller.action.animationTime).setEase(LeanTweenType.easeInOutQuint);

        boardPiece.Update_Location_Data(nextLocation);
    }
}
