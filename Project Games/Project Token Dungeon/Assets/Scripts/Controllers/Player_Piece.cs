using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAction
{
    void Action();
}

public enum aimDirection
{
    right, bottom, left, top
}

[System.Serializable]
public class Inserted_Token
{
    public Token_ScrObj insertedToken;
    public aimDirection aimDirection;
}

public class Player_Piece : MonoBehaviour
{
    public List<Inserted_Token> insertedTokens = new List<Inserted_Token>();
    public Action_Controller[] actionControllers;

    private void Start()
    {
        Play_Action();
    }

    public void Play_Action()
    {
        for (int i = 0; i < insertedTokens.Count; i++)
        {
            for (int j = 0; j < actionControllers.Length; j++)
            {
                if (insertedTokens[i].insertedToken != actionControllers[j].token) continue;

                if (!actionControllers[j].gameObject.TryGetComponent(out IAction action)) continue;
                
                action.Action();
                break;
            }
        }
        insertedTokens.Clear();
    }
}
