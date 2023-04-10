using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    private int _currentRowNum;
    public int currentRowNum { get => _currentRowNum; set => _currentRowNum = value; }

    private int _currentColumnNum;
    public int currentColumnNum { get => _currentColumnNum; set => _currentColumnNum = value; }

    public void Set_Data(int row, int column)
    {
        currentRowNum = row;
        currentColumnNum = column;
    }

    public void Move()
    {
        // lean tween to vector zero
        transform.localPosition = Vector2.zero;
    }
}
