using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAction
{
    void Action();
}

public enum aimDirection
{
    top, right, bottom, left
}

public class Action_Controller : MonoBehaviour
{
    public Player_Piece playerPiece;
    public Token_ScrObj token;
}