using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAction
{
    void Token_Action();
}

public class Player_Piece : MonoBehaviour
{
    public List<Token_ScrObj> insertedToken = new List<Token_ScrObj>();
}
