using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Controller : MonoBehaviour
{
    [HideInInspector] public Board_Piece boardPiece;
    public Action_ScrObj action;

    private void Awake()
    {
        Connect_BoardPiece();
    }

    private void Connect_BoardPiece()
    {
        boardPiece = transform.parent.parent.gameObject.GetComponent<Board_Piece>();
    }
}
