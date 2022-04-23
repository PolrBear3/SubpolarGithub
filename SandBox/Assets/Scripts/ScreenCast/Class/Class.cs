using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Class
{
    private string _typeName;
    public string typename { get { return _typeName; } set { _typeName = value; } }
    
    public enum againstType { rock, paper, scissors}

    public virtual void Result_Message(string typename, againstType type)
    {
        Debug.Log(typename + ": you tied");
    }
}

public class Rock : Class
{
    public override void Result_Message(string typename, againstType type)
    {
        if (type == againstType.paper)
        {
            Debug.Log(typename + ": you lost");
        }
        else if (type == againstType.scissors)
        {
            Debug.Log(typename + ": you won");
        }
        else
        {
            base.Result_Message(typename, type);
        }
    }
}

public class Paper : Class
{
    public override void Result_Message(string typename, againstType type)
    {
        if (type == againstType.rock)
        {
            Debug.Log(typename + ": you won");
        }
        else if (type == againstType.scissors)
        {
            Debug.Log(typename + ": you lost");
        }
        else
        {
            base.Result_Message(typename, type);
        }
    }
}

public class Scissors : Class
{
    public override void Result_Message(string typename, againstType type)
    {
        if (type == againstType.rock)
        {
            Debug.Log(typename + ": you lost");
        }
        else if (type == againstType.paper)
        {
            Debug.Log(typename + ": you won");
        }
        else
        {
            base.Result_Message(typename, type);
        }
    }
}