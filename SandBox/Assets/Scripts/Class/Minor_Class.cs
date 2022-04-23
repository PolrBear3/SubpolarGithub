using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minor_Class : MonoBehaviour
{
    Rock rock = new Rock();
    Paper paper = new Paper();
    Scissors scissors = new Scissors();

    void Rock_Result()
    {
        rock.Result_Message("rock", Class.againstType.scissors);
    }
    void Paper_Result()
    {
        paper.Result_Message("paper", Class.againstType.scissors);
    }
    void Scissors_Result()
    {
        scissors.Result_Message("scissors", Class.againstType.scissors);
    }

    public void Print_Result_Message()
    {
        Rock_Result();
        Paper_Result();
        Scissors_Result();
    }
}